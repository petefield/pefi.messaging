using System.Net.Http.Headers;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Channels;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace pefi.Rabbit;

    public class Rabbit
    {
        private ConnectionFactory _factory;

        public Rabbit(string host, string username, string password)
        {
            _factory = new ConnectionFactory() { HostName = host, UserName = username, Password = password };
        }

        public async Task<Topic> CreateTopic(string name)
        {
            using var connection = await _factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            await channel.ExchangeDeclareAsync(exchange: name, type: ExchangeType.Topic);
            return new Topic(_factory, name);
        }

    }

    public class Topic(ConnectionFactory ConnectionFactory, string ExchangeName) 
    {
        public async Task Publish(string key, string message)
        {
            using var connection = await ConnectionFactory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            var body = Encoding.UTF8.GetBytes(message);
            await channel.BasicPublishAsync(exchange: ExchangeName, routingKey: key, body: body);
            Console.WriteLine($"{key} {message}");
        }

        public async Task Subscribe(string filter, Func<string, string, Task> onReceived)
        {
            using var connection = await ConnectionFactory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

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
            Console.ReadLine();
        }
}