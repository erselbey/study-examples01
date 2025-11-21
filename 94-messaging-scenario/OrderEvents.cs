using System;

public record OrderCreatedEvent(Guid OrderId, string Email);
public record OrderPaidEvent(Guid OrderId, string Email);
public record OrderFailedEvent(Guid OrderId, string Email);
