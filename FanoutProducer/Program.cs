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
    durable: false,
    autoDelete: false
);

Console.WriteLine("Exchange 'fanout-exchange' creado!");
Console.WriteLine("Escribe mensajes (exit para salir):");

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
        body: body
    );

    Console.WriteLine($"📤 [FANOUT] Enviado: {mensaje}");
}