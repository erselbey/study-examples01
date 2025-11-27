using System.Text.Json.Serialization;
using Confluent.Kafka;
using Microsoft.AspNetCore.Http.Json;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

var serviceName = builder.Configuration["SERVICE_NAME"] ?? "messaging-gateway";
var environmentName = builder.Configuration["ENVIRONMENT"] ?? builder.Environment.EnvironmentName;
var kafkaBroker = builder.Configuration["KAFKA_BROKER"] ?? "localhost:9092";
var rabbitHost = builder.Configuration["RABBITMQ_HOST"] ?? "localhost";
var rabbitUser = builder.Configuration["RABBITMQ_USER"] ?? "guest";
var rabbitPassword = builder.Configuration["RABBITMQ_PASSWORD"] ?? "guest";

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.WriteIndented = true;
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

builder.Services.AddSingleton<IKafkaProducerFactory>(_ => new KafkaProducerFactory(kafkaBroker));
builder.Services.AddSingleton<IRabbitPublisherFactory>(_ => new RabbitPublisherFactory(rabbitHost, rabbitUser, rabbitPassword));

var app = builder.Build();

app.MapGet("/", () => Results.Json(new
{
    status = "ok",
    service = serviceName,
    env = environmentName,
    kafka = kafkaBroker,
    rabbit = rabbitHost
}));

app.MapGet("/health", () => Results.Json(new
{
    status = "ok",
    service = serviceName,
    env = environmentName,
    timestamp = DateTimeOffset.UtcNow
}));

app.MapPost("/publish/kafka", async (KafkaPublishRequest request, IKafkaProducerFactory factory) =>
{
    if (string.IsNullOrWhiteSpace(request.Topic))
    {
        return Results.BadRequest("Topic is required");
    }

    await using var producer = factory.Create();

    var message = new Message<string?, string?>
    {
        Key = request.Key,
        Value = request.Message
    };

    try
    {
        var result = await producer.ProduceAsync(request.Topic, message);
        return Results.Ok(new
        {
            service = serviceName,
            env = environmentName,
            topic = result.Topic,
            partition = result.Partition.Value,
            offset = result.Offset.Value
        });
    }
    catch (ProduceException<string?, string?> ex)
    {
        return Results.Problem($"Kafka error: {ex.Error.Reason}", statusCode: 502);
    }
});

app.MapPost("/publish/rabbit", (RabbitPublishRequest request, IRabbitPublisherFactory factory) =>
{
    if (string.IsNullOrWhiteSpace(request.Queue))
    {
        return Results.BadRequest("Queue is required");
    }

    using var connection = factory.CreateConnection();
    using var channel = connection.CreateModel();
    channel.QueueDeclare(queue: request.Queue, durable: false, exclusive: false, autoDelete: false);

    var body = System.Text.Encoding.UTF8.GetBytes(request.Message ?? string.Empty);
    channel.BasicPublish(exchange: string.Empty, routingKey: request.Queue, basicProperties: null, body: body);

    return Results.Ok(new
    {
        service = serviceName,
        env = environmentName,
        queue = request.Queue,
        published = true
    });
});

app.Run();

public record KafkaPublishRequest(string Topic, string? Message, string? Key);

public record RabbitPublishRequest(string Queue, string? Message);

public interface IKafkaProducerFactory
{
    IProducer<string?, string?> Create();
}

public sealed class KafkaProducerFactory : IKafkaProducerFactory
{
    private readonly ProducerConfig _config;

    public KafkaProducerFactory(string broker)
    {
        _config = new ProducerConfig
        {
            BootstrapServers = broker,
            Acks = Acks.All,
            EnableIdempotence = true
        };
    }

    public IProducer<string?, string?> Create() => new ProducerBuilder<string?, string?>(_config).Build();
}

public interface IRabbitPublisherFactory
{
    IConnection CreateConnection();
}

public sealed class RabbitPublisherFactory : IRabbitPublisherFactory
{
    private readonly ConnectionFactory _factory;

    public RabbitPublisherFactory(string host, string user, string password)
    {
        _factory = new ConnectionFactory
        {
            HostName = host,
            UserName = user,
            Password = password,
            DispatchConsumersAsync = true
        };
    }

    public IConnection CreateConnection() => _factory.CreateConnection();
}
