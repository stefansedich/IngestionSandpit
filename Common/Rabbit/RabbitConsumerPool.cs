using Akka.Actor;
using Akka.Event;

namespace Common.Rabbit
{
    public class RabbitConsumerPool : ReceiveActor
    {
        private readonly RabbitConsumerSettings _settings;
        private readonly ILoggingAdapter _log = Context.GetLogger();

        public RabbitConsumerPool(RabbitConsumerSettings settings)
        {
            _settings = settings;

            Start();
        }

        private void Start()
        {
            _log.Debug("Starting {0} consumers.", _settings.Streams);

            for (var i = 0; i < _settings.Streams; i++)
            {
                var props = RabbitConsumerProps.Create(_settings);
                Context.ActorOf(props);
            }
        }
    }

    public static class RabbitConsumerPoolProps
    {
        public static Props Create(RabbitConsumerSettings settings)
        {
            return Props.Create(() => new RabbitConsumerPool(settings));
        }
    }
}