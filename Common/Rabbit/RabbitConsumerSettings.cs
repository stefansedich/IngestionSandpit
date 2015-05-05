using Akka.Actor;

namespace Common.Rabbit
{
    public class RabbitConsumerSettings
    {
        public IActorRef Consumer { get; private set; }
        public IMessageSerializer Serializer { get; private set; }
        public string RabbitUri { get; private set; }
        public string Queue { get; private set; }
        public ushort Streams { get; private set; }
        public ushort PrefetchCount { get; set; }

        public RabbitConsumerSettings(
            IActorRef consumer, 
            IMessageSerializer serializer, 
            string rabbitUri, 
            string queue, 
            ushort streams = 1, 
            ushort prefetchCount = 512)
        {
            Consumer = consumer;
            Serializer = serializer;
            RabbitUri = rabbitUri;
            Queue = queue;
            Streams = streams;
            PrefetchCount = prefetchCount;
        }
    }
}