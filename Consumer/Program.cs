using System;
using Akka.Actor;
using Akka.Routing;
using Common;
using Common.Rabbit;
using Serilog;

namespace Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.ColoredConsole()
                .MinimumLevel.Debug()
                .CreateLogger();

            var system = ActorSystem.Create("rabbit-testing");
            StartRabbitConsumer(system);

            Console.WriteLine("Consumer running...");
            Console.ReadKey();
        }

        private static void StartRabbitConsumer(ActorSystem system)
        {
            var pool = new ConsistentHashingPool(4)
                .WithHashMapping(m =>
                {
                    var ingestEvent = m as RabbitMessageEnvelope<IngestEvent>;

                    return ingestEvent?.Message.CollectionId;
                });

            var routingConsumerPool = system.ActorOf(Props.Create<EventRoutingConsumer>().WithRouter(pool), "event-router-pool");
            var settings = new RabbitConsumerSettings(
                consumer: routingConsumerPool,
                serializer: new BsonMessageSerializer(),
                rabbitUri: "amqp://localhost/",
                queue: "events",
                streams: 8,
                prefetchCount: 512
                );

            var props = RabbitConsumerPoolProps.Create(settings);
            system.ActorOf(props);
        }
    }
}
