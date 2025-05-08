var Rabbit = new pefi.Rabbit.Rabbit("192.168.0.5", "username", "password");
var Topic = await Rabbit.CreateTopic("Events");

if(args.Length ==0)
{
    Console.WriteLine($"Running as producer");

    while(true)
    {
        await Topic.Publish("events.service.created","servicename");
    }
}
else{
    var k = args[0];

    Console.WriteLine($"Running as subscriber {k}");

    await Topic.Subscribe(k, async (key, message) => {
        Console.WriteLine("Message Received");
        await Task.Yield();
    });
}
    