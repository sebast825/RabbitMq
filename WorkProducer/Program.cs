using RabbitMQ.Client;
using System.Text;

Console.WriteLine("Work Producer started!");

var factory = new ConnectionFactory() { HostName = "localhost" };

using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

// Cola durable para Work Queues
await channel.QueueDeclareAsync(queue: "work_queue",
                     durable: true,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);

var messages = new[]
{
    "Task 1 - Easy",
    "Task 2 - Medium...",
    "Task 3 - Hard.........",
    "Task 4 - Very Hard............",
    "Task 5 - Extreme..................."
};

foreach (var message in messages)
{
    var body = Encoding.UTF8.GetBytes(message);

    var properties = new BasicProperties { Persistent = true };


    await channel.BasicPublishAsync(exchange: "",
                         routingKey: "work_queue",
                         body: body,
                         mandatory: false,
                         basicProperties: properties);

    Console.WriteLine($" [x] Sent: {message}");
}

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();