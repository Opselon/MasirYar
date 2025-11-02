// src/IdentityService/Tests/IdentityService.IntegrationTests/UsersControllerTests.cs
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Api;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;
using Xunit;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace IdentityService.IntegrationTests;

public class ApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:16")
        .WithDatabase("testdb")
        .WithUsername("testuser")
        .WithPassword("testpass")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Add AppSettings to the configuration
        var projectDir = Directory.GetCurrentDirectory();
        var configPath = Path.Combine(projectDir, "appsettings.Development.json");
        builder.ConfigureAppConfiguration((context, conf) =>
        {
            conf.AddJsonFile(configPath);
        });
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<IdentityDbContext>));
            services.AddDbContext<IdentityDbContext>(options =>
            {
                options.UseNpgsql(_dbContainer.GetConnectionString());
            });
            // Ensure the database is created
            var sp = services.BuildServiceProvider();
            using (var scope = sp.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<IdentityDbContext>();
                db.Database.EnsureCreated();
            }
        });
    }

    public async Task InitializeAsync() => await _dbContainer.StartAsync();
    public new async Task DisposeAsync() => await _dbContainer.DisposeAsync();
}


public class UsersControllerTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client;

    public UsersControllerTests(ApiFactory apiFactory)
    {
        _client = apiFactory.CreateClient();
    }

    [Fact]
    public async Task Register_ShouldReturnCreated_WhenPayloadIsValid()
    {
        // Arrange
        var request = new RegisterUserDto
        {
            Username = "integration_test_user",
            Email = "integration@test.com",
            Password = "Password123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/users/register", request);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var content = await response.Content.ReadFromJsonAsync<RegisterUserResponse>();
        Assert.NotNull(content);
        Assert.NotEqual(Guid.Empty, content.UserId);
    }
}

public class RegisterUserDto
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}

public class RegisterUserResponse
{
    public Guid UserId { get; set; }
}
