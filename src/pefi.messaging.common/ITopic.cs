
namespace pefi.Rabbit
{
    public interface ITopic : IDisposable
    {
        Task Publish(string key, string message);
        Task Subscribe(string filter, Func<string, string, Task> onReceived);

        Task Publish<T>(string key, T message);
        Task Subscribe<T>(string filter, Func<string, T, Task> onReceived);



    }
}