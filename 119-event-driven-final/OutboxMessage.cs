using System;

public class OutboxMessage
{
    public Guid Id { get; set; }
    public string EventType { get; set; } = default!;
    public string Payload { get; set; } = default!;
    public DateTime OccurredOn { get; set; }
    public DateTime? PublishedOn { get; set; }
}
