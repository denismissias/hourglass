using FluentAssertions;
using Hourglass.Models;
using Hourglass.Repository;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Hourglass.Tests.Repository;

/// <summary>
/// Tests for ProjectRepository
/// </summary>
public class ProjectRepositoryTests : IDisposable
{
    private readonly MySqlContext _dbContext;
    private readonly ProjectRepository _repository;

    public ProjectRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<MySqlContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new MySqlContext(options);
        _repository = new ProjectRepository(_dbContext);
    }

    [Fact]
    public async Task ExistsAsync_WithExistingProjectId_ReturnsTrue()
    {
        // Arrange
        var project = new Project
        {
            Title = "Test Project",
            Description = "A test project"
        };

        await _dbContext.Projects.AddAsync(project);
        await _dbContext.SaveChangesAsync();

        // Act
        var exists = await _repository.ExistsAsync(project.Id);

        // Assert
        exists.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_WithNonExistingProjectId_ReturnsFalse()
    {
        // Act
        var exists = await _repository.ExistsAsync(999);

        // Assert
        exists.Should().BeFalse();
    }

    [Fact]
    public async Task ExistsAsync_WithMultipleProjects_OnlyReturnsRequestedProject()
    {
        // Arrange
        var project1 = new Project { Title = "Project 1", Description = "Description 1" };
        var project2 = new Project { Title = "Project 2", Description = "Description 2" };
        var project3 = new Project { Title = "Project 3", Description = "Description 3" };

        await _dbContext.Projects.AddRangeAsync(project1, project2, project3);
        await _dbContext.SaveChangesAsync();

        // Act
        var exists1 = await _repository.ExistsAsync(project1.Id);
        var exists2 = await _repository.ExistsAsync(project2.Id);
        var exists3 = await _repository.ExistsAsync(project3.Id);
        var existsNonExistent = await _repository.ExistsAsync(999);

        // Assert
        exists1.Should().BeTrue();
        exists2.Should().BeTrue();
        exists3.Should().BeTrue();
        existsNonExistent.Should().BeFalse();
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}
