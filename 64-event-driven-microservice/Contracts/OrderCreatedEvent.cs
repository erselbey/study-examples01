using System;

public record OrderCreatedEvent(Guid OrderId, string CustomerId, decimal Total);
