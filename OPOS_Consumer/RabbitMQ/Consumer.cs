using Newtonsoft.Json;
using OPOS_project.Scheduler;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Collections.Immutable;
using System.Text;

using System.Text.Json;
using System.Diagnostics;
using System.IO;
using System.Printing.IndexedProperties;
using OPOS_Consumer;
using System.Windows;

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
            JobMessage message = JsonConvert.DeserializeObject<JobMessage>(jsonMessage);

            //Console.WriteLine($" [x] Received '{message.Name}'");
            JobType jobType = (JobType)message.JobType;

            MainWindow.setJobMessage(message); //sets the message to the appropriate MainWindow reference.

            Application.Current.Dispatcher.Invoke(() =>
            {
                MainWindow.updateNewTaskPlayerControl(MainWindow.currentJob);
            });

                channel.BasicAck(ea.DeliveryTag, false);
        };
    }

    public void ConsumeMessagesFromUnscheduled()
    {

        string consumeTag = channel.BasicConsume(queue: queueName1,
                                                 autoAck: false,
                                                 consumer: consumer);
    }

    public void ConsumeMessagesFromScheduled()
    {
       

        string consumeTag = channel.BasicConsume(queue: queueName2,
                                                 autoAck: false,
                                                 consumer: consumer);
    }



    public void Dispose()
    {
        channel.Close();
        connection.Close();
    }
}
