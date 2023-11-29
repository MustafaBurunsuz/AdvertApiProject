using advert.api.Services.abstractt;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace advert.api.Services.Concrete
{
    public class RabbitMqService : IRabbitMqService
    {
        private readonly string _hostName;
        private readonly string _queueName;
        private readonly string _userName;
        private readonly string _password;

        public RabbitMqService(string hostName, string queueName, string userName, string password)
        {
            _hostName = hostName;
            _queueName = queueName;
            _userName = userName;
            _password = password;
        }

        public void SendMessage(string message)
        {
            using (var connection = GetConnection())
            using (var channel = connection.CreateModel())
            {
                // Mesaj gönderme işlemleri
                channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "", routingKey: _queueName, basicProperties: null, body: body);
            }
        }

        public IConnection GetConnection()
        {
            var factory = new ConnectionFactory()
            {
                HostName = _hostName,
                UserName = "guest", 
                Password = "guest"
            };

            return factory.CreateConnection();
        }
    }
}
