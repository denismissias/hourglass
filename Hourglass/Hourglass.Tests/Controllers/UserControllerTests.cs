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
/// Tests for UserController
/// </summary>
public class UserControllerTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<ILogger<UserController>> _mockLogger;
    private readonly UserController _controller;

    public UserControllerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockLogger = new Mock<ILogger<UserController>>();

        _controller = new UserController(_mockUserRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Create_WithValidRequest_ReturnsCreatedAtAction()
    {
        // Arrange
        var request = new UserRequest
        {
            Login = "newuser",
            Email = "newuser@example.com",
            Name = "New User",
            Password = "password123"
        };

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
        var createdUser = new User
        {
            Id = 1,
            Login = request.Login,
            Email = request.Email,
            Name = request.Name,
            Password = hashedPassword
        };

        _mockUserRepository.Setup(r => r.ExistsAsync(request.Login))
            .ReturnsAsync(false);

        _mockUserRepository.Setup(r => r.CreateAsync(It.IsAny<User>()))
            .ReturnsAsync(createdUser);

        // Act
        var result = await _controller.Create(request);

        // Assert
        result.Result.Should().BeOfType<CreatedAtActionResult>();
        var createdResult = result.Result as CreatedAtActionResult;
        createdResult!.StatusCode.Should().Be((int)HttpStatusCode.Created);
        createdResult.Value.Should().NotBeNull();

        var userResponse = createdResult.Value as UserResponse;
        userResponse.Should().NotBeNull();

        _mockUserRepository.Verify(r => r.ExistsAsync(request.Login), Times.Once);
        _mockUserRepository.Verify(r => r.CreateAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task Create_WithDuplicateLogin_ReturnsBadRequest()
    {
        // Arrange
        var request = new UserRequest
        {
            Login = "existinguser",
            Email = "existing@example.com",
            Name = "Existing User",
            Password = "password123"
        };

        _mockUserRepository.Setup(r => r.ExistsAsync(request.Login))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.Create(request);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result.Result as BadRequestObjectResult;
        badRequestResult!.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);

        _mockUserRepository.Verify(r => r.CreateAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Update_WithValidRequest_ReturnsOk()
    {
        // Arrange
        var userId = 1;
        var request = new UserRequest
        {
            Login = "updateduser",
            Email = "updated@example.com",
            Name = "Updated User",
            Password = "newpassword123"
        };

        _mockUserRepository.Setup(r => r.ExistsAsync(userId))
            .ReturnsAsync(true);

        _mockUserRepository.Setup(r => r.UpdateAsync(It.IsAny<User>()))
            .ReturnsAsync(new User { Id = userId, Login = request.Login, Email = request.Email, Name = request.Name, Password = BCrypt.Net.BCrypt.HashPassword(request.Password) });

        // Act
        var result = await _controller.Update(userId, request);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.StatusCode.Should().Be((int)HttpStatusCode.OK);

        _mockUserRepository.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task Update_WithNonExistentUser_ReturnsNotFound()
    {
        // Arrange
        var userId = 999;
        var request = new UserRequest
        {
            Login = "user",
            Email = "user@example.com",
            Name = "User",
            Password = "password"
        };

        _mockUserRepository.Setup(r => r.ExistsAsync(userId))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.Update(userId, request);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = result.Result as NotFoundObjectResult;
        notFoundResult!.StatusCode.Should().Be((int)HttpStatusCode.NotFound);

        _mockUserRepository.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Get_WithExistingUserId_ReturnsOk()
    {
        // Arrange
        var userId = 1;
        var user = new User
        {
            Id = userId,
            Login = "testuser",
            Email = "test@example.com",
            Name = "Test User",
            Password = "hashed_password"
        };

        _mockUserRepository.Setup(r => r.GetAsync(userId))
            .ReturnsAsync(user);

        // Act
        var result = await _controller.Get(userId);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.StatusCode.Should().Be((int)HttpStatusCode.OK);

        var userResponse = okResult.Value as UserResponse;
        userResponse.Should().NotBeNull();

        _mockUserRepository.Verify(r => r.GetAsync(userId), Times.Once);
    }

    [Fact]
    public async Task Get_WithNonExistentUserId_ReturnsNotFound()
    {
        // Arrange
        var userId = 999;

        _mockUserRepository.Setup(r => r.GetAsync(userId))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _controller.Get(userId);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = result.Result as NotFoundObjectResult;
        notFoundResult!.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_WithNullRequest_ThrowsArgumentNullException()
    {
        // Act & Assert
        var act = () => _controller.Create(null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task Update_WithNullRequest_ThrowsArgumentNullException()
    {
        // Act & Assert
        var act = () => _controller.Update(1, null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }
}
