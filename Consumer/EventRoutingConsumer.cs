using Akka.Actor;
using Akka.Event;
using Common;
using Common.Rabbit;

namespace Consumer
{
    public class EventRoutingConsumer : ReceiveActor
    {
        private readonly ILoggingAdapter _log = Context.GetLogger();

        public EventRoutingConsumer()
        {
            _log.Info("Starting routing consumer {0}.", Context.Self.Path);
                
            Receive<RabbitMessageEnvelope<IngestEvent>>(m =>
            {
                var collectionId = m.Message.CollectionId;
                var consumer = Context.Child(collectionId);

                if (consumer == Nobody.Instance)
                {
                    consumer = Context.ActorOf(CollectionEventConsumerProps.Create(collectionId), collectionId);
                }
                
                consumer.Forward(m);
            });
        }
    }

    public static class EventRoutingConsumerProps
    {
        public static Props Create()
        {
            return Props.Create(() => new EventRoutingConsumer());
        }
    }
}