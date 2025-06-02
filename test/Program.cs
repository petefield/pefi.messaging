using var messageBroker = new pefi.Rabbit.MessageBroker("192.168.0.5", "username", "password", null);
using var topic = await messageBroker.CreateTopic("Events");


    Console.WriteLine($"Running as subscriber events.service.created");

    await topic.Subscribe("events.service.created", async (key, message) => {
        Console.WriteLine("Message Received");
        await Task.Yield();
    });

    Console.ReadLine();

    