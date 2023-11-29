using advert.api.Models;
using advert.api.Services.abstractt;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Text.Json;

namespace advert.api.Services.Concrete
{
    public class RabbitMqConsumer : IRabbitMqConsumer
    {
        private readonly IRabbitMqService _rabbitMqService;
        private readonly IAdvertRepository _advertRepository;

        public RabbitMqConsumer(IRabbitMqService rabbitMqService, IAdvertRepository advertRepository)
        {
            _rabbitMqService = rabbitMqService;
            _advertRepository = advertRepository;
        }

        public void Consume()
        {
            var connection = _rabbitMqService.GetConnection();
            var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "myQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                var advertVisit = JsonSerializer.Deserialize<AdvertVisit>(message);

                // AdvertVisit'i veritabanına ekler
                _advertRepository.InsertAdvertVisitAsync(advertVisit);

                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            channel.BasicConsume(queue: "myQueue", autoAck: false, consumer: consumer);
        }
    }
}
