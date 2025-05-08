
namespace pefi.Rabbit
{
    public interface ITopic : IDisposable
    {
        Task Publish(string key, string message);
        Task Subscribe(string filter, Func<string, string, Task> onReceived);
    }
}