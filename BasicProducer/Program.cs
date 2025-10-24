using RabbitMQ.Client;
using System.Text;

Console.WriteLine("Basic Producer started!");

var factory = new ConnectionFactory() { HostName = "localhost" };

using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

await channel.QueueDeclareAsync(queue: "hello",
                     durable: false,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);


for (int i = 0; i < 5; i++)
{
     string message = "Hello World! " + i;

    var body = Encoding.UTF8.GetBytes(message);

    await channel.BasicPublishAsync(exchange: "",
                         routingKey: "hello",
                         body: body);
    Console.WriteLine($" [x] Sent {message}");

}

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();