using System;

public record OrderCreatedEvent(Guid OrderId, decimal Total);
public record PaymentRequestedEvent(Guid OrderId, decimal Total);
public record PaymentCompletedEvent(Guid OrderId);
public record PaymentFailedEvent(Guid OrderId);
public record StockDecreaseRequestedEvent(Guid OrderId);
public record OrderCompletedEvent(Guid OrderId);
