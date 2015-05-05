namespace Common.Rabbit
{
    public interface IRabbitMessageEnvelope
    {
        ulong DeliveryTag { get; }
    }
}