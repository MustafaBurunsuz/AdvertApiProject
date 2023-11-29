using RabbitMQ.Client;

namespace advert.api.Services.abstractt
{
    public interface IRabbitMqService
    {
        void SendMessage(string message);
        IConnection GetConnection(); 
    }
}