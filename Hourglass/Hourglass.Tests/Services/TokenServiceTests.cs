using FluentAssertions;
using Hourglass.Configuration;
using Hourglass.Models;
using Hourglass.Services;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Xunit;

namespace Hourglass.Tests.Services;

/// <summary>
/// Tests for TokenService
/// </summary>
public class TokenServiceTests
{
    private readonly JwtSettings _jwtSettings;
    private readonly TokenService _tokenService;

    public TokenServiceTests()
    {
        _jwtSettings = new JwtSettings
        {
            Secret = "test-secret-key-1234567890123456789012345678",
            ExpirationHours = 2,
            Issuer = "TestHourglass"
        };

        _tokenService = new TokenService(_jwtSettings);
    }

    [Fact]
    public void Generate_WithValidUser_ReturnsValidJwtToken()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Name = "Test User",
            Email = "test@example.com",
            Login = "testuser",
            Password = "hashed_password"
        };

        // Act
        var token = _tokenService.Generate(user);

        // Assert
        token.Should().NotBeNullOrEmpty();
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                System.Text.Encoding.ASCII.GetBytes(_jwtSettings.Secret)),
            ValidateIssuer = true,
            ValidIssuer = _jwtSettings.Issuer,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        };

        tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
        validatedToken.Should().NotBeNull();
    }

    [Fact]
    public void Generate_WithValidUser_ContainsCorrectClaims()
    {
        // Arrange
        var user = new User
        {
            Id = 42,
            Name = "John Doe",
            Email = "john@example.com",
            Login = "johndoe",
            Password = "hashed_password"
        };

        // Act
        var token = _tokenService.Generate(user);

        // Assert
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);

        // JWT uses "unique_name" for ClaimTypes.Name, "email" for email, and "nameid" for NameIdentifier
        jwtToken.Claims.Should().Contain(c => c.Type == "unique_name" && c.Value == user.Name);
        jwtToken.Claims.Should().Contain(c => c.Type == "email" && c.Value == user.Email);
        jwtToken.Claims.Should().Contain(c => c.Type == "nameid" && c.Value == user.Id.ToString());
        jwtToken.Issuer.Should().Be(_jwtSettings.Issuer);
    }

    [Fact]
    public void Generate_WithValidUser_TokenExpiresAtCorrectTime()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Name = "Test User",
            Email = "test@example.com",
            Login = "testuser",
            Password = "hashed_password"
        };

        var beforeGeneration = DateTime.UtcNow;

        // Act
        var token = _tokenService.Generate(user);

        var afterGeneration = DateTime.UtcNow;

        // Assert
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);

        var expectedExpiration = beforeGeneration.AddHours(_jwtSettings.ExpirationHours);
        var timeDifference = Math.Abs((jwtToken.ValidTo - expectedExpiration).TotalSeconds);

        timeDifference.Should().BeLessThan(2); // Allow 2 second tolerance
    }

    [Fact]
    public void Generate_WithNullUser_ThrowsArgumentNullException()
    {
        // Act & Assert
        var act = () => _tokenService.Generate(null!);

        act.Should().Throw<ArgumentNullException>();
    }
}
