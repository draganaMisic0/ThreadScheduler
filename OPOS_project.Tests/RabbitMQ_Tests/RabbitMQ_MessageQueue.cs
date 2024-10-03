using RabbitMQ.Client;

namespace OPOS_project.Tests.RabbitMQ_Tests
{
    public class RabbitMQ_MessageQueue
    {
        private IConnection connection;
        private IModel channel;
        private readonly string queueName1 = "unscheduled-jobs";
        private readonly string queueName2 = "scheduled-jobs";
        private MessageQueue instance = null;

        [Fact]
        public void RabbitMQ_Connection_Exists()
        {

            

            string exchangeName = "OPOSExchange";
            string routingKey1 = "lab-routing-key1";
            string routingKey2 = "lab-routing-key2";


            ConnectionFactory factory = new ConnectionFactory();

            factory.Uri = new Uri("amqp://guest:guest@localhost:5672");
            factory.ClientProvidedName = "Scheduler App";

            try
            {
            connection = factory.CreateConnection();
            }
            catch(Exception ex) {
                
            }

            Assert.NotNull(connection);
            channel = connection.CreateModel();

            channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
            channel.QueueDeclare(queueName1, false, false, false, null);
            channel.QueueBind(queueName1, exchangeName, routingKey1, null);

            channel.QueueDeclare(queueName2, false, false, false, null);
            channel.QueueBind(queueName2, exchangeName, routingKey2, null);
        }
    }
}