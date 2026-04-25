using FluentAssertions;
using Hourglass.Models;
using Microsoft.Extensions.Logging;
using Hourglass.Repository;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Hourglass.Tests.Repository;

/// <summary>
/// Tests for TimeRepository
/// </summary>
public class TimeRepositoryTests : IDisposable
{
    private readonly MySqlContext _dbContext;
    private readonly Mock<ILogger<TimeRepository>> _mockLogger;
    private readonly TimeRepository _repository;

    public TimeRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<MySqlContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new MySqlContext(options);
        _mockLogger = new Mock<ILogger<TimeRepository>>();
        _repository = new TimeRepository(_dbContext, _mockLogger.Object);
    }

    [Fact]
    public async Task CreateAsync_WithValidTime_CreatesTimeInDatabase()
    {
        // Arrange
        var time = new Time
        {
            ProjectId = 1,
            UserId = 1,
            StartedAt = DateTime.UtcNow,
            EndedAt = DateTime.UtcNow.AddHours(2)
        };

        // Act
        var result = await _repository.CreateAsync(time);

        // Assert
        result.Id.Should().BeGreaterThan(0);
        var savedTime = await _dbContext.Times.FindAsync(result.Id);
        savedTime.Should().NotBeNull();
        savedTime!.ProjectId.Should().Be(1);
        savedTime.UserId.Should().Be(1);
    }

    [Fact]
    public async Task CreateAsync_WithNullTime_ThrowsArgumentNullException()
    {
        // Act & Assert
        var act = () => _repository.CreateAsync(null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task GetAsync_WithExistingProjectId_ReturnsTimeEntriesForProject()
    {
        // Arrange
        var time1 = new Time
        {
            ProjectId = 10,
            UserId = 1,
            StartedAt = DateTime.UtcNow,
            EndedAt = DateTime.UtcNow.AddHours(1)
        };

        var time2 = new Time
        {
            ProjectId = 10,
            UserId = 2,
            StartedAt = DateTime.UtcNow.AddDays(-1),
            EndedAt = DateTime.UtcNow.AddDays(-1).AddHours(2)
        };

        var time3 = new Time
        {
            ProjectId = 11,
            UserId = 1,
            StartedAt = DateTime.UtcNow,
            EndedAt = DateTime.UtcNow.AddHours(1)
        };

        await _dbContext.Times.AddRangeAsync(time1, time2, time3);
        await _dbContext.SaveChangesAsync();

        // Act
        var results = await _repository.GetAsync(10);

        // Assert
        results.Should().HaveCount(2);
        results.Should().AllSatisfy(t => t.ProjectId.Should().Be(10));
    }

    [Fact]
    public async Task GetAsync_WithNonExistingProjectId_ReturnsEmptyList()
    {
        // Act
        var results = await _repository.GetAsync(999);

        // Assert
        results.Should().BeEmpty();
    }

    [Fact]
    public async Task UpdateAsync_WithExistingTime_UpdatesTime()
    {
        // Arrange
        var time = new Time
        {
            ProjectId = 1,
            UserId = 1,
            StartedAt = DateTime.UtcNow,
            EndedAt = DateTime.UtcNow.AddHours(1)
        };

        await _dbContext.Times.AddAsync(time);
        await _dbContext.SaveChangesAsync();

        var newEndTime = DateTime.UtcNow.AddHours(3);
        time.EndedAt = newEndTime;

        // Act
        var result = await _repository.UpdateAsync(time);

        // Assert
        result.EndedAt.Should().Be(newEndTime);

        var savedTime = await _dbContext.Times.FindAsync(time.Id);
        savedTime!.EndedAt.Should().Be(newEndTime);
    }

    [Fact]
    public async Task UpdateAsync_WithNullTime_ThrowsArgumentNullException()
    {
        // Act & Assert
        var act = () => _repository.UpdateAsync(null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}
