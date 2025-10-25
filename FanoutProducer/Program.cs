using RabbitMQ.Client;
using System.Text;

Console.WriteLine("📢 Fanout Producer started!");

// Create a connection factory pointing to the local RabbitMQ server
var factory = new ConnectionFactory() { HostName = "localhost" };

// Create the connection and channel
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

// Create a FANOUT exchange (no queue directly attached here)
await channel.ExchangeDeclareAsync(
    exchange: "fanout-exchange",
    type: ExchangeType.Fanout,
    durable: true,
    autoDelete: false
);

Console.WriteLine("Exchange 'fanout-exchange' created successfully!");
Console.WriteLine("Type your messages below (type 'exit' to quit):");

// Make message persistent (so it survives broker restarts)
var properties = new BasicProperties { Persistent = true };

while (true)
{
    Console.Write("> ");
    var message = Console.ReadLine();

    if (message?.ToLower() == "exit") break;
    if (string.IsNullOrWhiteSpace(message)) continue;

    var body = Encoding.UTF8.GetBytes(message);

    // Publish to the EXCHANGE (not to a specific queue)
    await channel.BasicPublishAsync(
        exchange: "fanout-exchange",
        routingKey: "",  // Empty for fanout type
        body: body,
        mandatory: false,
        basicProperties: properties
    );

    Console.WriteLine($"📤 [FANOUT] Sent: {message}");
}

Console.WriteLine("👋 Producer stopped.");
