using FluentAssertions;
using Hourglass.Controllers;
using Microsoft.Extensions.Logging;
using Hourglass.Models;
using Hourglass.Repository.Interfaces;
using Hourglass.Request;
using Hourglass.Response;
using Hourglass.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;
using Xunit;

namespace Hourglass.Tests.Controllers;

/// <summary>
/// Tests for AuthenticationController
/// </summary>
public class AuthenticationControllerTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<ITokenService> _mockTokenService;
    private readonly Mock<ILogger<AuthenticationController>> _mockLogger;
    private readonly AuthenticationController _controller;

    public AuthenticationControllerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockTokenService = new Mock<ITokenService>();
        _mockLogger = new Mock<ILogger<AuthenticationController>>();

        _controller = new AuthenticationController(
            _mockUserRepository.Object,
            _mockTokenService.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task Authenticate_WithValidCredentials_ReturnsOkWithToken()
    {
        // Arrange
        var request = new AuthenticationRequest
        {
            Login = "testuser",
            Password = "password123"
        };

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
        var user = new User
        {
            Id = 1,
            Login = "testuser",
            Email = "test@example.com",
            Name = "Test User",
            Password = hashedPassword
        };

        const string token = "jwt-token-123";

        _mockUserRepository.Setup(r => r.GetAsync(request.Login))
            .ReturnsAsync(user);

        _mockTokenService.Setup(s => s.Generate(user))
            .Returns(token);

        // Act
        var result = await _controller.Authenticate(request);

        // Assert
        result.Value.Should().NotBeNull();
        var response = result.Value as AuthenticationResponse;
        response.Should().NotBeNull();
        response!.Token.Should().Be(token);
        response.User.Should().NotBeNull();

        _mockUserRepository.Verify(r => r.GetAsync(request.Login), Times.Once);
        _mockTokenService.Verify(s => s.Generate(user), Times.Once);
    }

    [Fact]
    public async Task Authenticate_WithInvalidPassword_ReturnsBadRequest()
    {
        // Arrange
        var request = new AuthenticationRequest
        {
            Login = "testuser",
            Password = "wrongpassword"
        };

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("correctpassword");
        var user = new User
        {
            Id = 1,
            Login = "testuser",
            Email = "test@example.com",
            Name = "Test User",
            Password = hashedPassword
        };

        _mockUserRepository.Setup(r => r.GetAsync(request.Login))
            .ReturnsAsync(user);

        // Act
        var result = await _controller.Authenticate(request);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result.Result as BadRequestObjectResult;
        badRequestResult!.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Authenticate_WithNonExistentUser_ReturnsBadRequest()
    {
        // Arrange
        var request = new AuthenticationRequest
        {
            Login = "nonexistent",
            Password = "password123"
        };

        _mockUserRepository.Setup(r => r.GetAsync(request.Login))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _controller.Authenticate(request);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result.Result as BadRequestObjectResult;
        badRequestResult!.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);

        _mockTokenService.Verify(s => s.Generate(It.IsAny<User>()), Times.Never);
    }

    [Theory]
    [InlineData("")]
    public async Task Authenticate_WithEmptyLogin_ReturnsBadRequest(string login)
    {
        // Arrange
        var request = new AuthenticationRequest
        {
            Login = login,
            Password = "password123"
        };

        _controller.ModelState.AddModelError("Login", "Login is required");

        // Act
        var result = await _controller.Authenticate(request);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Authenticate_WithNullRequest_ThrowsArgumentNullException()
    {
        // Act & Assert
        var act = () => _controller.Authenticate(null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }
}
