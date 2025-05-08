using var messageBroker = await pefi.Rabbit.MessageBroker.Create("192.168.0.5", "username", "password");
using var topic = await messageBroker.CreateTopic("Events");

if(args.Length ==0)
{
    Console.WriteLine($"Running as producer");

    while(true)
    {
        await topic.Publish("events.service.created","servicename");
    }
}
else{
    var k = args[0];

    Console.WriteLine($"Running as subscriber {k}");

    await topic.Subscribe(k, async (key, message) => {
        Console.WriteLine("Message Received");
        await Task.Yield();
    });

    Console.ReadLine();
}
    