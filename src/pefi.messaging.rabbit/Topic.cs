using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace pefi.Rabbit;

public class Topic(IChannel channel, string ExchangeName) : IDisposable, ITopic
{
    public void Dispose()
    {
        channel.Dispose();
    }

    public async Task Publish(string key, string message)
    {
        var body = Encoding.UTF8.GetBytes(message);
        await channel.BasicPublishAsync(exchange: ExchangeName, routingKey: key, body: body);
        Console.WriteLine($"{key} {message}");
    }

    public async Task Subscribe(string filter, Func<string, string, Task> onReceived)
    {
        // declare a server-named queue
        QueueDeclareOk queueDeclareResult = await channel.QueueDeclareAsync();
        string queueName = queueDeclareResult.QueueName;

        await channel.QueueBindAsync(queue: queueName, exchange: ExchangeName, routingKey: filter);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var routingKey = ea.RoutingKey;
            await onReceived(routingKey, message);
        };

        await channel.BasicConsumeAsync(queueName, autoAck: true, consumer: consumer);
    }
}