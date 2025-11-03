using Microsoft.Extensions.Logging;
using Moq;
using NotificationService.Application.IntegrationEvents.EventHandling;
using NotificationService.Application.IntegrationEvents.Events;
using Xunit;

namespace NotificationService.UnitTests.EventHandling;

public class UserRegisteredEventHandlerTests
{
    [Fact]
    public async Task Handle_ShouldLogWelcomeMessage_WhenEventIsReceived()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<UserRegisteredEventHandler>>();
        var handler = new UserRegisteredEventHandler(loggerMock.Object);
        var @event = new UserRegisteredIntegrationEvent(Guid.NewGuid(), "test@example.com");

        // Act
        await handler.Handle(@event);

        // Assert
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Sending welcome notification to test@example.com")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }
}
