using System;

namespace Common.Rabbit
{
    public static class RabbitMessageEnvelope
    {
        public static IRabbitMessageEnvelope Create(ulong deliveryTag, object message)
        {
            var messageType = message.GetType();
            var contextType = typeof (RabbitMessageEnvelope<>)
                .MakeGenericType(messageType);

            return (IRabbitMessageEnvelope) Activator.CreateInstance(contextType, deliveryTag, message);
        }    
    }

    public class RabbitMessageEnvelope<TMessage> : IRabbitMessageEnvelope
        where TMessage : class
    {
        public ulong DeliveryTag { get; }
        public TMessage Message { get; }

        public RabbitMessageEnvelope(ulong deliveryTag, TMessage message)
        {
            DeliveryTag = deliveryTag;
            Message = message;
        }
    }
}