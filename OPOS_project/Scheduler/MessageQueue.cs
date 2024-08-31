using OPOS_project.Scheduler;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Newtonsoft.Json; // You'll need to add the Newtonsoft.Json NuGet package for serialization

public class MessageQueue : IDisposable
{
    private readonly IConnection connection;
    private readonly IModel channel;
    private const string QueueName = "jobs_queue";

    public MessageQueue()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        connection = factory.CreateConnection();
        channel = connection.CreateModel();
        channel.QueueDeclare(queue: QueueName,
                             durable: false,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);
    }

    public void PublishMessage(JobMessage message)
    {
        var jsonMessage = JsonConvert.SerializeObject(message);
        var body = Encoding.UTF8.GetBytes(jsonMessage);
        channel.BasicPublish(exchange: "",
                             routingKey: QueueName,
                             basicProperties: null,
                             body: body);
        Console.WriteLine($" [x] Sent '{message.Name}'");
    }

    // Method to consume messages from the queue
    public void ConsumeMessages()
    {
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var jsonMessage = Encoding.UTF8.GetString(body);
            var message = JsonConvert.DeserializeObject<JobMessage>(jsonMessage);

            if (message != null)
            {
                Console.WriteLine($" [x] Received '{message.Name}'");
                ProcessMessage(message);
            }
        };
        channel.BasicConsume(queue: QueueName,
                             autoAck: true,
                             consumer: consumer);
    }

    private int CalculatePriority(JobMessage jobMessage)
    {
        if (!jobMessage.StartDateAndTime.HasValue && !jobMessage.Deadline.HasValue)
        {
            // Non-timed jobs, highest priority
            return int.MaxValue;
        }
        else
        {
            // Timed jobs: Priority based on time remaining until the deadline
            var timeRemaining = (jobMessage.Deadline.Value - DateTime.Now).TotalMinutes;
            return (int)(timeRemaining / (jobMessage.TotalExecutionTime ?? 1));
        }
    }

    private Job ProcessMessage(JobMessage message)
    {

        int calculatedPriority = CalculatePriority(message);
        Job createdJob;
        try
        {
            createdJob = JobFactory.createJob(message, calculatedPriority);
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return null;
        }

        return createdJob;
        
    }

    public void Dispose()
    {
        channel.Close();
        connection.Close();
    }
}
