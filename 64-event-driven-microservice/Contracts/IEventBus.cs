using System.Threading.Tasks;

public interface IEventBus
{
    Task PublishAsync<T>(T @event) where T : class;
}
