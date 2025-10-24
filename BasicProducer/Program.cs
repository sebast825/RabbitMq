using RabbitMQ.Client;
using System.Text;

Console.WriteLine("Basic Producer iniciado!");

var factory = new ConnectionFactory() { HostName = "localhost" };

// En versiones recientes se usa ConnectAsync o CreateConnection de esta forma:
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

await channel.QueueDeclareAsync(queue: "hello",
                     durable: false,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);

const string message = "Hello World!";
var body = Encoding.UTF8.GetBytes(message);

await channel.BasicPublishAsync(exchange: "",
                     routingKey: "hello",
                     body: body);

Console.WriteLine($" [x] Sent {message}");

Console.WriteLine(" Presiona [enter] para salir.");
Console.ReadLine();