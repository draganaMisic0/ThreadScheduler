using Newtonsoft.Json;
using OPOS_project.Scheduler;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Collections.Immutable;
using System.Text;
using OPOS_Consumer.RabbitMQ;
using System.Text.Json;
using System.Diagnostics;

public class Consumer : IDisposable
{
    private readonly IConnection connection;
    private readonly IModel channel;
    private readonly string queueName1 = "unscheduled-jobs";
    private readonly string queueName2 = "scheduled-jobs";
    EventingBasicConsumer consumer = null;

    public Consumer()
    {
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

        channel.BasicQos(0, 1, false);

        this.consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var jsonMessage = Encoding.UTF8.GetString(body);
            var message = JsonConvert.DeserializeObject<JobMessage>(jsonMessage);

            Console.WriteLine($" [x] Received '{message.Name}'");
            JobType jobType = (JobType)message.JobType;

            // Based on the job type, start the corresponding process
            Console.WriteLine("ulazi");
            //StartJobProcess(jobType, message);

            // Process the message (you can implement your processing logic here)
            // ProcessMessage(message);
            channel.BasicAck(ea.DeliveryTag, false);
        };
    }

    public void PublishMessage(JobMessage message)
    {
        var jsonMessage = JsonConvert.SerializeObject(message);
        var body = Encoding.UTF8.GetBytes(jsonMessage);
        channel.BasicPublish(exchange: "",
                             routingKey: queueName1,
                             basicProperties: null,
                             body: body);

        Console.WriteLine($" [x] Sent '{message.Name}'");

    }

    public void ConsumeMessagesFromUnscheduled()
    {

        channel.BasicConsume(queue: queueName1,
                             autoAck: false,
                             consumer: consumer);
    }

    public void ConsumeMessagesFromScheduled()
    {
       

        channel.BasicConsume(queue: queueName2,
                             autoAck: false,
                             consumer: consumer);
    }
    
        private void StartJobProcess(JobType jobType, JobMessage message)
    {
        // Determine the executable or script to run based on the job type
        string executablePath = GetExecutablePathForJob(jobType);

        // Create the process start info
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = executablePath,
            Arguments = $"--jobType {jobType} --jobName {message.Name} -- bitmapPath {message.BitmapPath}",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        // Start the process
        using (Process process = Process.Start(startInfo))
        {
            // Optionally, read the output or handle the process
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
        }
    }

    private string GetExecutablePathForJob(JobType jobType)
    {
        string baseDirectory = @"C:\Users\WIN11\Desktop\OPOS_project\OPOS_project\OPOS_JobExecutor\bin\Debug\net8.0";
        string executablePath = jobType switch
        {
            JobType.Blur => Path.Combine(baseDirectory, "OPOS_JobExecutor.exe"),
            JobType.DetectEdges => Path.Combine(baseDirectory, "OPOS_JobExecutor.exe"),
            JobType.Embossing => Path.Combine(baseDirectory, "OPOS_JobExecutor.exe"),
            JobType.Sharpen => Path.Combine(baseDirectory, "OPOS_JobExecutor.exe"),
            _ => throw new ArgumentException("Unknown job type")
        };

        if (!File.Exists(executablePath))
        {
            throw new FileNotFoundException("Executable not found", executablePath);
        }

        return executablePath;
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

   /* private Job ProcessMessage(JobMessage message)
    {
        int calculatedPriority = CalculatePriority(message);
        Job createdJob;
        try
        {
            createdJob = JobFactory.createJob(message, calculatedPriority);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating job: {ex.Message}");
            return null;
        }
       

        return createdJob;
    }
   */

    public void Dispose()
    {
        channel.Close();
        connection.Close();
    }
}
