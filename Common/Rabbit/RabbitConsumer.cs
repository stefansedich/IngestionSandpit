using System;
using Akka.Actor;
using Akka.Event;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Common.Rabbit
{
    public class RabbitConsumer : ReceiveActor
    {
        private const int DequeueTimeout = 500;

        private readonly RabbitConsumerSettings _settings;
        private readonly ILoggingAdapter _log = Context.GetLogger();
        private IConnection _connection;
        private IModel _channel;
        private QueueingBasicConsumer _consumer;
        
        public RabbitConsumer(RabbitConsumerSettings settings)
        {
            _settings = settings;
            
            Receiving();
        }

        protected override void PreStart()
        {
            var factory = new ConnectionFactory
            {
                Uri = _settings.RabbitUri
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _consumer = new QueueingBasicConsumer(_channel);

            _channel.BasicQos(0, _settings.PrefetchCount, false);
            _channel.BasicConsume(_settings.Queue, false, _consumer);
            _log.Debug("Started consumer listening to {0} on queue {1} with prefetch size {2}.", _settings.RabbitUri, _settings.Queue, _settings.PrefetchCount);
        }

        protected override void PostStop()
        {
            _channel.Dispose();
            _connection.Dispose();
        }

        private void Receiving()
        {
            Receive<ReceiveNext>(m =>
            {
                BasicDeliverEventArgs result;
                if (_consumer.Queue.Dequeue(DequeueTimeout, out result))
                {
                    try
                    {
                        var message = _settings.Serializer.Deserialize(result.Body);
                        var envelope = RabbitMessageEnvelope.Create(result.DeliveryTag, message);

                        _settings.Consumer.Tell(envelope);
                    }
                    catch (Exception ex)
                    {
                        _log.Error(ex, "Failed to deserialize message.");
                        _channel.BasicAck(result.DeliveryTag, false);
                    }
                }

                Self.Tell(new ReceiveNext());
            });

            Receive<Processed>(m =>
            {
                _log.Debug("Message {0} processed successfully.", m.DeliveryTag);
                _channel.BasicAck(m.DeliveryTag, false);
            });

            Self.Tell(new ReceiveNext());
        }

        private class ReceiveNext
        {
        }

        public class Processed
        {
            public ulong DeliveryTag { get; }

            public Processed(IRabbitMessageEnvelope messageContext)
            {
                DeliveryTag = messageContext.DeliveryTag;
            }
        }
    }

    public static class RabbitConsumerProps
    {
        public static Props Create(RabbitConsumerSettings settings)
        {
            return Props.Create(() => new RabbitConsumer(settings));
        }
    }
}