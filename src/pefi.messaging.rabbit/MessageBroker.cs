using RabbitMQ.Client;

namespace pefi.Rabbit;

public class MessageBroker : IMessageBroker
{
    private readonly IConnection? _connection;

    public MessageBroker(string host, string username, string password)
    {
        var factory = new ConnectionFactory() { HostName = host, UserName = username, Password = password };

        _connection = factory.CreateConnectionAsync().Result;
    }
 
    public async Task<ITopic> CreateTopic(string name)
    {
        var channel = await _connection!.CreateChannelAsync();

        await channel.ExchangeDeclareAsync(exchange: name, type: ExchangeType.Topic);
        return new Topic(channel, name);
    }

    public void Dispose()
    {
        _connection?.Dispose();
    }
}
