using Akka.Actor;
using Akka.Event;
using Common;
using Common.Rabbit;

namespace Consumer
{
    public class CollectionEventConsumer : ReceiveActor
    {
        private readonly ILoggingAdapter _log = Context.GetLogger();
        
        public CollectionEventConsumer(string collectionId)
        {
            _log.Info("Starting event consumer for collection {0} with path {1}.", collectionId, Self.Path);

            Receive<RabbitMessageEnvelope<IngestEvent>>(m =>
            {
                _log.Debug("Ingesting event {0} for collection {1}.", m.Message.Type, m.Message.CollectionId);
                
                Sender.Tell(new RabbitConsumer.Processed(m));
            });
        }
    }

    public static class CollectionEventConsumerProps
    {
        public static Props Create(string collectionId)
        {
            return Props.Create(() => new CollectionEventConsumer(collectionId));
        }
    }
}