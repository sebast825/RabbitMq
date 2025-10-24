# Hello World con RabbitMQ en C#

This project is a simple Hello World example using RabbitMQ in C#.
It contains two programs:

- `BasicProducer.cs` → Sends 5 messages to the hello queue 
- `BasicConsumer.cs` → Receives messages from the hello queue

> Note: If you open multiple BasicConsumer instances, the messages will be distributed between them using RabbitMQ's round-robin load balancing.


---

## Requirements

- [RabbitMQ](https://www.rabbitmq.com/download.html) running on localhost (port 5672)
> The management interface (optional) is available on port 15672


---

## How to Run

1. Start RabbitMQ (Docker recommended):


```bash
# Make sure it's running on port 15672
docker run -it --rm --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:4-management

```
2. In one console, run the BasicConsumer project:

```bash
dotnet run --project ./BasicConsumer/BasicConsumer.csproj

```
3. In another console, run the BasicProducer project:

```bash
dotnet run --project ./BasicProducer/BasicProducer.csproj

```


