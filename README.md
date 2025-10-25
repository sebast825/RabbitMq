# RabbitMQ Patterns in C#

## Objective
Show the evolution from basic to production-ready RabbitMQ implementations.

---

# Patterns

## 1. Hello World
**Use Case**: Learning & prototyping  
**Characteristics**: Fire-and-forget, no delivery guarantees

**Programs:**
- `BasicProducer.cs` → Sends messages to `hello` queue
- `BasicConsumer.cs` → Receives messages from `hello` queue

**Key Limitations ❌**
- Messages lost if consumer crashes
- No persistence (volatile queues)
- Uncontrolled message distribution

## 2. Work Queues  
**Use Case**: Background job processing, load distribution  
**Characteristics**: Delivery guarantees, crash recovery

**Programs:**
- `WorkProducer.cs` → Sends tasks to `work_queue`
- `WorkWorker.cs` → Processes tasks from `work_queue`

**Key Features ✅**
- Messages re-delivered if worker crashes
- Durable queues & persistent messages
- Fair distribution with QoS control

### Simulate a Worker Crash in Work Queues:
```bash
if (message.Contains("Task 3")) 
{
    Console.WriteLine("💥 Worker crashed before ACK!");
    Environment.Exit(1);
}

#This line only executes for successful processing:
await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
```


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

### Hello World

```bash
# Terminal 1 - Consumer
dotnet run --project BasicConsumer/

# Terminal 2 - Producer  
dotnet run --project BasicProducer/
```

### Work Queues

```bash
# Terminal 1 - Worker (first instance)
dotnet run --project WorkWorker/

# Terminal 2 - Worker (second instance)
dotnet run --project WorkWorker/

# Terminal 3 - Producer
dotnet run --project WorkProducer/
```

