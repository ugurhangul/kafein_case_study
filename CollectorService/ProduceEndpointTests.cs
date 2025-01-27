using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace CollectorService;

public class ProduceEndpointTests
{
    [Fact]
    public async Task Produce_ReturnsOk_WhenMessageIsProduced()
    {
        // Arrange
        var mockKafkaProducer = new Mock<IKafkaProducer>();
        mockKafkaProducer
            .Setup(p => p.ProduceAsync("audit-events", It.IsAny<AuditEvent>()))
            .ReturnsAsync("partition-0-offset-1");

        var auditEvent = new AuditEvent
        {
            EventId = "123",
            EventType = "TestEvent",
            Timestamp = "2023-10-10T14:00:00Z",
            Username = "testuser",
            DatabaseName = "testdb",
            Statement = "SELECT * FROM Test",
            Severity = "High"
        };

        var endpoint = new Func<AuditEvent, IKafkaProducer, Task<IResult>>(async (eventToLog, producer) =>
        {
            try
            {
                var result = await producer.ProduceAsync("audit-events", eventToLog);
                return Results.Ok($"Event sent to Kafka: {result}");
            }
            catch (Exception ex)
            {
                return Results.Problem($"Error sending message to Kafka: {ex.Message}");
            }
        });

        // Act
        var result = await endpoint(auditEvent, mockKafkaProducer.Object);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Event sent to Kafka: partition-0-offset-1", okResult.Value);
    }

    [Fact]
    public async Task Produce_ReturnsProblem_WhenKafkaFails()
    {
        // Arrange
        var mockKafkaProducer = new Mock<IKafkaProducer>();
        mockKafkaProducer
            .Setup(p => p.ProduceAsync("audit-events", It.IsAny<AuditEvent>()))
            .ThrowsAsync(new Exception("Kafka unavailable"));

        var auditEvent = new AuditEvent
        {
            EventId = "123",
            EventType = "TestEvent",
            Timestamp = "2023-10-10T14:00:00Z",
            Username = "testuser",
            DatabaseName = "testdb",
            Statement = "SELECT * FROM Test",
            Severity = "High"
        };

        var endpoint = new Func<AuditEvent, IKafkaProducer, Task<IResult>>(async (eventToLog, producer) =>
        {
            try
            {
                var result = await producer.ProduceAsync("audit-events", eventToLog);
                return Results.Ok($"Event sent to Kafka: {result}");
            }
            catch (Exception ex)
            {
                return Results.Problem($"Error sending message to Kafka: {ex.Message}");
            }
        });

        // Act
        var result = await endpoint(auditEvent, mockKafkaProducer.Object);

        // Assert
        var problemDetails = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, problemDetails.StatusCode);
        Assert.Contains("Kafka unavailable", problemDetails.Value.ToString());
    }
}