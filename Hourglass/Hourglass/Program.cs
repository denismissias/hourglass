using Hourglass.Configuration;
using Hourglass.Endpoints;
using Hourglass.Repository;
using Hourglass.Repository.Interfaces;
using Hourglass.Services;
using Hourglass.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Microsoft.OpenApi;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, _, _) =>
    {
        document.Info = new()
        {
            Title = "Hourglass API",
            Version = "v1",
            Description = "Time tracking API built with ASP.NET Minimal APIs."
        };

        document.Servers =
        [
            new() { Url = "/", Description = "Default server" }
        ];

        document.Components ??= new();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "Provide JWT token in Authorization header as: Bearer {token}."
        };

        return Task.CompletedTask;
    });
});

builder.Services.AddCors();

var connection = builder.Configuration.GetConnectionString("Default");

var serverVersion = new MySqlServerVersion(new Version(8, 0, 33));

builder.Services.AddDbContext<MySqlContext>(
            dbContextOptions =>
            {
                dbContextOptions.UseMySql(connection, serverVersion);

                if (builder.Environment.IsDevelopment())
                {
                    dbContextOptions
                        .LogTo(Console.WriteLine, LogLevel.Debug)
                        .EnableSensitiveDataLogging()
                        .EnableDetailedErrors();
                }
            });

var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>() 
    ?? throw new InvalidOperationException("Jwt settings are not configured");

builder.Services.AddSingleton(jwtSettings);

var key = Encoding.ASCII.GetBytes(jwtSettings.Secret);
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidateAudience = false
    };
});

builder.Services.AddAuthorization();

builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<ITimeRepository, TimeRepository>();
builder.Services.AddTransient<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseExceptionHandler();
app.UseStatusCodePages();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapHourglassEndpoints();

app.MapGet("/health", () => TypedResults.Ok(new { status = "ok" }))
    .WithName("HealthCheck")
    .WithSummary("Health check")
    .WithDescription("Returns API liveness status.")
    .Produces<Ok<object>>(StatusCodes.Status200OK);

app.Run();
