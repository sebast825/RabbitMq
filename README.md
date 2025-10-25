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


## 3. Fanout Pattern
**Use Case:** Broadcast messages to multiple consumers
**Characteristics:** Each message delivered to ALL bound queues

### Key Configuration
- **Durable Exchange:** `durable: true` - survives broker restarts  
- **Persistent Messages:** `properties.Persistent = true` - saved to disk

### Queue Configuration Examples

#### Persistent Queue (Consumer B)

```bash
var queue = await channel.QueueDeclareAsync(
    queue: "fanout-consumerB-queue",  // Explicit name
    durable: true,                    // Survives broker restarts
    exclusive: false,                 // Multiple consumers can connect
    autoDelete: false                 // Not automatically deleted
);
);
```
**Characteristics:**

- Messages persist after RabbitMQ restarts
- Multiple instances can connect to the same queue
- Ideal for production services

#### Temporary Queue (Consumer A)

```bash
var queueName = (await channel.QueueDeclareAsync()).QueueName;  // Auto-generated name
```

**Characteristics:**

- Queue deleted when consumer disconnects
- Pending messages lost during restarts
- Useful for debugging/temporary scenarios

  

## Key Differences

| Aspect | Hello World | Work Queues | Fanout Pattern |
|--------|-------------|-------------|----------------|
| **Message Routing** | Direct to queue | Direct to queue | Broadcast to all bound queues |
| **Durability** | `false` | `true` | Configurable (persistent/temporary) |
| **Acknowledgments** | `autoAck: true` | `autoAck: false` + manual Ack | Configurable per consumer |
| **Message Persistence** | No | `Persistent = true` | `Persistent = true` (recommended) |
| **Quality of Service** | No control | `BasicQos(prefetchCount: 1)` | Configurable per consumer |
| **Crash Recovery** | ❌ Messages lost | ✅ Messages re-delivered | ✅ With persistent queues |
| **Scalability** | Single consumer | Multiple competing consumers | Multiple parallel consumers |
| **Use Case** | Development & testing | Background job processing | Event broadcasting & notifications |


###  When to Use Which?
- **Hello World**: Learning, prototyping, non-critical data
- **Work Queues**: Orders, user registrations, payment processing
- **Fanout**: Real-time notifications, event broadcasting, multiple subscribers



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

