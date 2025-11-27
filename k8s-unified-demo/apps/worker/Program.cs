using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;

var builder = WebApplication.CreateBuilder(args);

var serviceName = builder.Configuration["SERVICE_NAME"] ?? "worker";
var environmentName = builder.Configuration["ENVIRONMENT"] ?? builder.Environment.EnvironmentName;
var apiToken = builder.Configuration["API_TOKEN"] ?? "not-set";

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.WriteIndented = true;
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

var recentTasks = new List<WorkerTaskResponse>();

var app = builder.Build();

app.MapGet("/", () => Results.Json(new
{
    status = "ok",
    service = serviceName,
    env = environmentName,
    recentCount = recentTasks.Count
}));

app.MapGet("/health", () => Results.Json(new
{
    status = "ok",
    service = serviceName,
    env = environmentName,
    timestamp = DateTimeOffset.UtcNow
}));

app.MapGet("/tasks", () => Results.Ok(recentTasks));

app.MapPost("/tasks", (TaskPayload payload) =>
{
    if (string.IsNullOrWhiteSpace(payload.Job))
    {
        return Results.BadRequest("Job is required");
    }

    var processed = new WorkerTaskResponse(payload.Job, payload.Note, DateTimeOffset.UtcNow, apiToken);
    recentTasks.Add(processed);

    if (recentTasks.Count > 50)
    {
        recentTasks.RemoveAt(0);
    }

    return Results.Ok(new
    {
        service = serviceName,
        env = environmentName,
        handled = processed
    });
});

app.Run();

public record TaskPayload(string Job, string? Note);

public record WorkerTaskResponse(string Job, string? Note, DateTimeOffset ProcessedAt, string TokenUsed);
