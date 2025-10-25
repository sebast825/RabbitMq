using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

Console.WriteLine("👂 Fanout Consumer B iniciado!");  // ← Cambia aquí

var factory = new ConnectionFactory() { HostName = "localhost" };

using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

await channel.ExchangeDeclareAsync(
    exchange: "fanout-exchange",
    type: ExchangeType.Fanout,
    durable: false,
    autoDelete: false
);

var queueName = (await channel.QueueDeclareAsync(exclusive: true)).QueueName;
await channel.QueueBindAsync(
    queue: queueName,
    exchange: "fanout-exchange",
    routingKey: ""
);

Console.WriteLine($"✅ Conectado. Cola: {queueName}");
Console.WriteLine("🔄 Esperando mensajes...");

var consumer = new AsyncEventingBasicConsumer(channel);
consumer.ReceivedAsync += async (model, ea) =>
{
    var body = ea.Body.ToArray();
    var mensaje = Encoding.UTF8.GetString(body);
    Console.WriteLine($"📥 [Consumer B] Recibido: {mensaje}");  // ← Y aquí
};

await channel.BasicConsumeAsync(
    queue: queueName,
    autoAck: true,
    consumer: consumer
);

Console.WriteLine("Press Enter para salir.");
Console.ReadLine();