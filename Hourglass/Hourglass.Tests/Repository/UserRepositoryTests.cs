using FluentAssertions;
using Hourglass.Models;
using Microsoft.Extensions.Logging;
using Hourglass.Repository;
using Hourglass.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Hourglass.Tests.Repository;

/// <summary>
/// Tests for UserRepository
/// </summary>
public class UserRepositoryTests : IDisposable
{
    private readonly MySqlContext _dbContext;
    private readonly Mock<ILogger<UserRepository>> _mockLogger;
    private readonly UserRepository _repository;

    public UserRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<MySqlContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new MySqlContext(options);
        _mockLogger = new Mock<ILogger<UserRepository>>();
        _repository = new UserRepository(_dbContext, _mockLogger.Object);
    }

    [Fact]
    public async Task CreateAsync_WithValidUser_CreatesUserInDatabase()
    {
        // Arrange
        var user = new User
        {
            Login = "testuser",
            Email = "test@example.com",
            Name = "Test User",
            Password = "hashed_password"
        };

        // Act
        var result = await _repository.CreateAsync(user);

        // Assert
        result.Id.Should().BeGreaterThan(0);
        var savedUser = await _dbContext.Users.FindAsync(result.Id);
        savedUser.Should().NotBeNull();
        savedUser!.Login.Should().Be(user.Login);
    }

    [Fact]
    public async Task CreateAsync_WithNullUser_ThrowsArgumentNullException()
    {
        // Act & Assert
        var act = () => _repository.CreateAsync(null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task ExistsAsync_WithExistingLogin_ReturnsTrue()
    {
        // Arrange
        var user = new User
        {
            Login = "existinguser",
            Email = "existing@example.com",
            Name = "Existing User",
            Password = "hashed_password"
        };
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        // Act
        var exists = await _repository.ExistsAsync("existinguser");

        // Assert
        exists.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_WithNonExistingLogin_ReturnsFalse()
    {
        // Act
        var exists = await _repository.ExistsAsync("nonexistent");

        // Assert
        exists.Should().BeFalse();
    }

    [Fact]
    public async Task ExistsAsync_WithExistingUserId_ReturnsTrue()
    {
        // Arrange
        var user = new User
        {
            Login = "user1",
            Email = "user1@example.com",
            Name = "User One",
            Password = "hashed_password"
        };
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        // Act
        var exists = await _repository.ExistsAsync(user.Id);

        // Assert
        exists.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_WithNonExistingUserId_ReturnsFalse()
    {
        // Act
        var exists = await _repository.ExistsAsync(999);

        // Assert
        exists.Should().BeFalse();
    }

    [Fact]
    public async Task GetAsync_WithExistingLogin_ReturnsUser()
    {
        // Arrange
        var user = new User
        {
            Login = "getuser",
            Email = "getuser@example.com",
            Name = "Get User",
            Password = "hashed_password"
        };
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetAsync("getuser");

        // Assert
        result.Should().NotBeNull();
        result!.Login.Should().Be("getuser");
        result.Email.Should().Be("getuser@example.com");
    }

    [Fact]
    public async Task GetAsync_WithNonExistingLogin_ReturnsNull()
    {
        // Act
        var result = await _repository.GetAsync("nonexistent");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAsync_WithExistingUserId_ReturnsUser()
    {
        // Arrange
        var user = new User
        {
            Login = "getbyid",
            Email = "getbyid@example.com",
            Name = "Get By ID User",
            Password = "hashed_password"
        };
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetAsync(user.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(user.Id);
        result.Login.Should().Be("getbyid");
    }

    [Fact]
    public async Task GetAsync_WithNonExistingUserId_ReturnsNull()
    {
        // Act
        var result = await _repository.GetAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_WithExistingUser_UpdatesUser()
    {
        // Arrange
        var user = new User
        {
            Login = "updateuser",
            Email = "updateuser@example.com",
            Name = "Update User",
            Password = "hashed_password"
        };
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        user.Name = "Updated Name";
        user.Email = "updated@example.com";

        // Act
        var result = await _repository.UpdateAsync(user);

        // Assert
        result.Name.Should().Be("Updated Name");
        result.Email.Should().Be("updated@example.com");

        var savedUser = await _dbContext.Users.FindAsync(user.Id);
        savedUser!.Name.Should().Be("Updated Name");
    }

    [Fact]
    public async Task UpdateAsync_WithNullUser_ThrowsArgumentNullException()
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
