using System;
using System.Threading.Tasks;

public class FakeEventBus : IEventBus
{
    public Task PublishAsync<T>(T @event) where T : class
    {
        Console.WriteLine($"Published {typeof(T).Name}: {@event}");
        return Task.CompletedTask;
    }
}
