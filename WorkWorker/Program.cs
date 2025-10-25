using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

Console.WriteLine("Work Worker started!");

var factory = new ConnectionFactory() { HostName = "localhost" };

using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

await channel.QueueDeclareAsync(queue: "work_queue",
                     durable: true,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);

// Configure Quality of Service (QoS) for fair message distribution
// This ensures a worker doesn't get overwhelmed with too many messages
channel.BasicQos(
    prefetchSize: 0,      // No limit on message size (0 = unlimited)
    prefetchCount: 1,     // Maximum 1 unacknowledged message per worker
    global: false         // Apply per consumer, not globally across connection
);
// WITHOUT BasicQos (problem):
// - RabbitMQ sends messages to consumers as soon as they're ready
// - If one worker gets long-running tasks, it may accumulate messages
// - Faster workers sit idle while slower workers get overwhelmed

// WITH BasicQos (solution):
// - Each worker gets only 1 message at a time
// - Worker must acknowledge current message before receiving next one
// - Ensures fair distribution: busy workers don't get new messages
// - Ideal for tasks with different processing times

var consumer = new AsyncEventingBasicConsumer(channel);

consumer.ReceivedAsync += async (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    
    Console.WriteLine($" [x] Received: {message}");

    // simulate crash in message 3
    if (message.Contains("Task 3"))
    {
        Console.WriteLine("💥 WORKER CRASHED!");
        Environment.Exit(1);  // Crash 
    }

    await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
};

await channel.BasicConsumeAsync(queue: "work_queue",
 // Ack manual, in case is true when the app crash te other messagges will be lost, because all were sent here (if only 1 instance was open)
                     autoAck: false, 
                     consumer: consumer);

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();