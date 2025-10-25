using RabbitMQ.Client;
using System.Text;

Console.WriteLine("📢 Fanout Producer iniciado!");

var factory = new ConnectionFactory() { HostName = "localhost" };

using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

// Crear un exchange de tipo FANOUT (no cola)
await channel.ExchangeDeclareAsync(
    exchange: "fanout-exchange",
    type: ExchangeType.Fanout,
    durable: true,
    autoDelete: false
);

Console.WriteLine("Exchange 'fanout-exchange' creado!");
Console.WriteLine("Escribe mensajes (exit para salir):");

var properties = new BasicProperties { Persistent = true };

while (true)
{
    Console.Write("> ");
    var mensaje = Console.ReadLine();

    if (mensaje?.ToLower() == "exit") break;
    if (string.IsNullOrWhiteSpace(mensaje)) continue;

    var body = Encoding.UTF8.GetBytes(mensaje);

    // Publicar al EXCHANGE (no a cola específica)
    await channel.BasicPublishAsync(
        exchange: "fanout-exchange",
        routingKey: "",  // Vacío para fanout
        body: body,
        mandatory: false,
        basicProperties: properties

    );

    Console.WriteLine($"📤 [FANOUT] Enviado: {mensaje}");
}