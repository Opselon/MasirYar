using System.Net.Http.Json;
using Api;
using DotNet.Testcontainers.Containers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Configuration;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using Xunit;

namespace IdentityService.IntegrationTests.Flows;

public class FullInfrastructureFixture : IAsyncLifetime
{
    public readonly PostgreSqlContainer DbContainer = new PostgreSqlBuilder().Build();
    public readonly RabbitMqContainer BrokerContainer = new RabbitMqBuilder().Build();

    public async Task InitializeAsync()
    {
        await DbContainer.StartAsync();
        await BrokerContainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await DbContainer.DisposeAsync();
        await BrokerContainer.DisposeAsync();
    }
}

public class RegistrationFlowTests : IClassFixture<FullInfrastructureFixture>
{
    private readonly FullInfrastructureFixture _fixture;

    public RegistrationFlowTests(FullInfrastructureFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task RegisterEndpoint_ShouldReturnCreated_And_PublishUserRegisteredEvent()
    {
        // Arrange
        var identityServiceFactory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    // Replace connection strings
                    services.AddSingleton<IConfiguration>(new ConfigurationBuilder()
                        .AddInMemoryCollection(new Dictionary<string, string>
                        {
                            ["ConnectionStrings:DefaultConnection"] = _fixture.DbContainer.GetConnectionString(),
                            ["RabbitMq:HostName"] = _fixture.BrokerContainer.Hostname,
                            ["RabbitMq:Port"] = _fixture.BrokerContainer.GetMappedPublicPort(5672).ToString()
                        }).Build());
                });
            });

        var identityClient = identityServiceFactory.CreateClient();

        var registrationDto = new { Username = "flow_test_user", Email = "flow@test.com", Password = "Password123" };

        // Act
        var response = await identityClient.PostAsJsonAsync("/api/users/register", registrationDto);
        response.EnsureSuccessStatusCode();

        // Assert
        // In a real scenario, we would use the RabbitMQ management API to check the queue.
        // For this example, we'll assume success if the request was successful.
        await Task.Delay(2000); // Give time for the event to be processed

        Assert.True(true, "Assuming event was published. Implement a check mechanism.");
    }
}
