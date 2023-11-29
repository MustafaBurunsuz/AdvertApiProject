using System;

namespace advert.api.Services.abstractt
{
    public interface IRabbitMqConsumer
    {
        void Consume();
    }
}