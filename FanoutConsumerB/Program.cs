using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

Console.WriteLine("👂 Fanout Consumer B started!");

// Create a connection factory pointing to the local RabbitMQ server
var factory = new ConnectionFactory() { HostName = "localhost" };

// Create the connection and channel
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

// Declare a FANOUT exchange (must match the producer's exchange)
await channel.ExchangeDeclareAsync(
    exchange: "fanout-exchange",
    type: ExchangeType.Fanout,
    durable: true,       // Survives broker restarts
    autoDelete: false
);

// ✅ Declare a named queue for this consumer
var queue = await channel.QueueDeclareAsync(
    queue: "fanout-consumerB-queue6",  // Fixed queue name
    durable: true,
    exclusive: false,
    autoDelete: false
);

var queueName = queue.QueueName;

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

    Console.WriteLine($"📥 [Consumer B] Received: {message}");
};

// Start consuming messages
await channel.BasicConsumeAsync(
    queue: queueName,
    autoAck: true,   // Automatically acknowledge messages
    consumer: consumer
);

Console.WriteLine("Press Enter to exit.");
Console.ReadLine();
