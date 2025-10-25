using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

Console.WriteLine("👂 Fanout Consumer B iniciado!");  

var factory = new ConnectionFactory() { HostName = "localhost" };

using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

await channel.ExchangeDeclareAsync(
    exchange: "fanout-exchange",
    type: ExchangeType.Fanout,
    durable: true,                    // Sobrevive a reinicios
    autoDelete: false
);

// BIEN ✅ - Usar QueueDeclareAsync con queue name explícito
var queue = await channel.QueueDeclareAsync(
    queue: "fanout-consumerB-queue",  // ← NOMBRE FIJO
    durable: true,
    exclusive: false,
    autoDelete: false
);
var queueName = queue.QueueName;

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

    Console.WriteLine($"📥 [Consumer B] Recibido: {mensaje}");
};

await channel.BasicConsumeAsync(
    queue: queueName,
    autoAck: true,
    consumer: consumer
);

Console.WriteLine("Press Enter para salir.");
Console.ReadLine();