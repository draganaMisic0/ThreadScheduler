using OPOS_project.Scheduler;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Immutable; // You'll need to add the Newtonsoft.Json NuGet package for serialization

public class MessageQueue : IDisposable
{
    private readonly IConnection connection;
    private readonly IModel channel;
    private readonly string queueName1 = "unscheduled-jobs";
    private readonly string queueName2 = "scheduled-jobs";
    private static MessageQueue instance = null;
    public static MessageQueue getInstance()
    {
        if (instance == null) { 
         new MessageQueue(); 
        }
        return instance;

    }


    private MessageQueue()
    {
        instance = this;

        string exchangeName = "OPOSExchange";
        string routingKey1 = "lab-routing-key1";
        string routingKey2 = "lab-routing-key2";
        

        ConnectionFactory factory = new ConnectionFactory();

        factory.Uri = new Uri("amqp://guest:guest@localhost:5672");
        factory.ClientProvidedName = "Scheduler App";

        connection = factory.CreateConnection();
        channel = connection.CreateModel();

        channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
        channel.QueueDeclare(queueName1, false, false, false, null);
        channel.QueueBind(queueName1, exchangeName, routingKey1, null);

        channel.QueueDeclare(queueName2, false, false, false, null);
        channel.QueueBind(queueName2, exchangeName, routingKey2, null);
    }

    public void PublishMessageToUnscheduled(JobMessage message)
    {
        var jsonMessage = JsonConvert.SerializeObject(message);
        var body = Encoding.UTF8.GetBytes(jsonMessage);
        channel.BasicPublish(exchange: "",
                             routingKey: queueName1,
                             basicProperties: null,
                             body: body);
        Console.WriteLine($" [x] Sent '{message.Name}'");
    }

    public void PublishMessageToScheduled(JobMessage message)
    {
        var jsonMessage = JsonConvert.SerializeObject(message);
        var body = Encoding.UTF8.GetBytes(jsonMessage);
        channel.BasicPublish(exchange: "",
                             routingKey: queueName2,
                             basicProperties: null,
                             body: body);
        Console.WriteLine($" [x] Sent '{message.Name}'");
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
