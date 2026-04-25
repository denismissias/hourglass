using FluentAssertions;
using Hourglass.Controllers;
using Microsoft.Extensions.Logging;
using Hourglass.Models;
using Hourglass.Repository.Interfaces;
using Hourglass.Request;
using Hourglass.Response;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;
using Xunit;

namespace Hourglass.Tests.Controllers;

/// <summary>
/// Tests for TimeController
/// </summary>
public class TimeControllerTests
{
    private readonly Mock<ITimeRepository> _mockTimeRepository;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IProjectRepository> _mockProjectRepository;
    private readonly Mock<ILogger<TimeController>> _mockLogger;
    private readonly TimeController _controller;

    public TimeControllerTests()
    {
        _mockTimeRepository = new Mock<ITimeRepository>();
        _mockUserRepository = new Mock<IUserRepository>();
        _mockProjectRepository = new Mock<IProjectRepository>();
        _mockLogger = new Mock<ILogger<TimeController>>();

        _controller = new TimeController(
            _mockTimeRepository.Object,
            _mockUserRepository.Object,
            _mockProjectRepository.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task Create_WithValidRequest_ReturnsCreatedAtAction()
    {
        // Arrange
        var request = new TimeRequest
        {
            ProjectId = 1,
            UserId = 1,
            StartedAt = DateTime.UtcNow,
            EndedAt = DateTime.UtcNow.AddHours(2)
        };

        var createdTime = new Time
        {
            Id = 1,
            ProjectId = request.ProjectId,
            UserId = request.UserId,
            StartedAt = request.StartedAt,
            EndedAt = request.EndedAt
        };

        _mockUserRepository.Setup(r => r.ExistsAsync(request.UserId))
            .ReturnsAsync(true);

        _mockProjectRepository.Setup(r => r.ExistsAsync(request.ProjectId))
            .ReturnsAsync(true);

        _mockTimeRepository.Setup(r => r.CreateAsync(It.IsAny<Time>()))
            .ReturnsAsync(createdTime);

        // Act
        var result = await _controller.Create(request);

        // Assert
        result.Result.Should().BeOfType<CreatedAtActionResult>();
        var createdResult = result.Result as CreatedAtActionResult;
        createdResult!.StatusCode.Should().Be((int)HttpStatusCode.Created);

        _mockUserRepository.Verify(r => r.ExistsAsync(request.UserId), Times.Once);
        _mockProjectRepository.Verify(r => r.ExistsAsync(request.ProjectId), Times.Once);
        _mockTimeRepository.Verify(r => r.CreateAsync(It.IsAny<Time>()), Times.Once);
    }

    [Fact]
    public async Task Create_WithNonExistentUser_ReturnsNotFound()
    {
        // Arrange
        var request = new TimeRequest
        {
            ProjectId = 1,
            UserId = 999,
            StartedAt = DateTime.UtcNow,
            EndedAt = DateTime.UtcNow.AddHours(2)
        };

        _mockUserRepository.Setup(r => r.ExistsAsync(request.UserId))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.Create(request);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = result.Result as NotFoundObjectResult;
        notFoundResult!.StatusCode.Should().Be((int)HttpStatusCode.NotFound);

        _mockProjectRepository.Verify(r => r.ExistsAsync(It.IsAny<int>()), Times.Never);
        _mockTimeRepository.Verify(r => r.CreateAsync(It.IsAny<Time>()), Times.Never);
    }

    [Fact]
    public async Task Create_WithNonExistentProject_ReturnsNotFound()
    {
        // Arrange
        var request = new TimeRequest
        {
            ProjectId = 999,
            UserId = 1,
            StartedAt = DateTime.UtcNow,
            EndedAt = DateTime.UtcNow.AddHours(2)
        };

        _mockUserRepository.Setup(r => r.ExistsAsync(request.UserId))
            .ReturnsAsync(true);

        _mockProjectRepository.Setup(r => r.ExistsAsync(request.ProjectId))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.Create(request);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = result.Result as NotFoundObjectResult;
        notFoundResult!.StatusCode.Should().Be((int)HttpStatusCode.NotFound);

        _mockTimeRepository.Verify(r => r.CreateAsync(It.IsAny<Time>()), Times.Never);
    }

    [Fact]
    public async Task Create_WithInvalidTimeRange_ValidatesRequest()
    {
        // Arrange
        var request = new TimeRequest
        {
            ProjectId = 1,
            UserId = 1,
            StartedAt = DateTime.UtcNow,
            EndedAt = DateTime.UtcNow.AddHours(-1) // End time before start time
        };

        _controller.ModelState.AddModelError("EndedAt", "End time must be after start time");

        // Act
        var result = await _controller.Create(request);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Create_WithNullRequest_ThrowsArgumentNullException()
    {
        // Act & Assert
        var act = () => _controller.Create(null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Create_WithInvalidUserId_ReturnsBadRequest(int userId)
    {
        // Arrange
        var request = new TimeRequest
        {
            ProjectId = 1,
            UserId = userId,
            StartedAt = DateTime.UtcNow,
            EndedAt = DateTime.UtcNow.AddHours(1)
        };

        _controller.ModelState.AddModelError("UserId", "User ID must be greater than 0");

        // Act
        var result = await _controller.Create(request);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }
}
