
namespace pefi.Rabbit
{
    public interface IMessageBroker : IDisposable
    {
        Task<ITopic> CreateTopic(string name);
    }
}