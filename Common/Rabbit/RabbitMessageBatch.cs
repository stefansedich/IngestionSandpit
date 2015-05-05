namespace Common.Rabbit
{
    public class RabbitMessageBatch<TMessage>
        where TMessage : class
    {
        public RabbitMessageEnvelope<TMessage> Messages { get; private set; }

        public RabbitMessageBatch(RabbitMessageEnvelope<TMessage> messages)
        {
            Messages = messages;
        }
    }
}