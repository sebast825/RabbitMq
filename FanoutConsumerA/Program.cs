using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

Console.WriteLine("👂 Fanout Consumer A started!");

var factory = new ConnectionFactory() { HostName = "localhost" };

// Create the connection and channel
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

// Declare the same FANOUT exchange (must match the producer)
await channel.ExchangeDeclareAsync(
    exchange: "fanout-exchange",
    type: ExchangeType.Fanout,
    durable: true,
    autoDelete: false
);

// Create a TEMPORARY and UNIQUE queue for this consumer
var queueName = (await channel.QueueDeclareAsync(durable: true)).QueueName;

// Bind the queue to the fanout exchange
await channel.QueueBindAsync(
    queue: queueName,
    exchange: "fanout-exchange",
    routingKey: ""  // Empty for fanout
);

Console.WriteLine($"✅ Connected. Queue: {queueName}");
Console.WriteLine("🔄 Waiting for messages...");

// Create the consumer and handle received messages
var consumer = new AsyncEventingBasicConsumer(channel);
consumer.ReceivedAsync += async (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"📥 [Consumer A] Received: {message}");
};

// Start consuming messages
await channel.BasicConsumeAsync(
    queue: queueName,
    autoAck: true,
    consumer: consumer
);

Console.WriteLine("Press Enter to exit.");
Console.ReadLine();
