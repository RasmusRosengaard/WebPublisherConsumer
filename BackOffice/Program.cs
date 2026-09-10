using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

const string HostName = "localhost";
const string ExchangeName = "tours";
const string RoutingKey = "tour.#"; // gets all tour.something
const string QueueName = "backoffice-service";

// creates a connection to the RabbitMQ server
var factory = new ConnectionFactory { HostName = HostName };
await using var connection = await factory.CreateConnectionAsync();
await using var channel = await connection.CreateChannelAsync();

// declares the exchange with the specified name and type
await channel.ExchangeDeclareAsync(ExchangeName, ExchangeType.Topic);

// declares this service's own queue
await channel.QueueDeclareAsync(QueueName, durable: true, exclusive: false, autoDelete: false);

// binds the queue to the exchange, so it gets every tour event
await channel.QueueBindAsync(QueueName, ExchangeName, RoutingKey);

Console.WriteLine($" [*] Waiting for {RoutingKey}. To exit press CTRL+C");

// handles each message as it arrives
var consumer = new AsyncEventingBasicConsumer(channel);
consumer.ReceivedAsync += (model, ea) =>
{
    var message = Encoding.UTF8.GetString(ea.Body.Span);
    Console.WriteLine($" {QueueName} Received '{ea.RoutingKey}': {message}");
    return Task.CompletedTask;
};

// starts consuming from the queue
await channel.BasicConsumeAsync(QueueName, autoAck: true, consumer: consumer);

// keeps the app running, otherwise it would exit and stop consuming
Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();
