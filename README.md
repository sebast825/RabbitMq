# RabbitMQ Patterns in C#

## Objective
Show the evolution from basic to production-ready RabbitMQ implementations.

## Patterns

### 1. Hello World (Basic Producer/Consumer)
- **Use Case**: Learning, development only
- **Characteristics**: Fire-and-forget, no guarantees
- **Code**: `/HelloWorld/`

### 2. Work Queues (Task Distribution)  
- **Use Case**: Background job processing, load distribution
- **Characteristics**: Delivery guarantees, crash recovery
- **Code**: `/WorkQueues/`

## Key Differences

| Aspect | Hello World | Work Queues |
|--------|-------------|-------------|
| **Durability** | `false` | `true` |
| **Acknowledgments** | `autoAck: true` | `autoAck: false` + manual Ack |
| **Message Persistence** | No | `Persistent = true` |
| **Quality of Service** | No control | `BasicQos(prefetchCount: 1)` |
| **Crash Recovery** | ❌ Messages lost | ✅ Messages re-delivered |
| **Use Case** | Development | Production |

##  When to Use Which?
- **Hello World**: Learning, prototyping, non-critical data
- **Work Queues**: Orders, user registrations, payment processing

  

## Hello World with RabbitMQ in C#

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


