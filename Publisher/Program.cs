using System;
using System.Linq;
using Common;
using Common.Rabbit;
using RabbitMQ.Client;
using Serilog;

namespace Publisher
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.ColoredConsole()
                .MinimumLevel.Debug()
                .CreateLogger();

            PublishMessages();
        }

        private static void PublishMessages()
        {
            Log.Information("Publishing messages...");

            var factory = new ConnectionFactory();
            var serializer = new BsonMessageSerializer();
            var rnd = new Random((int) DateTime.Now.Ticks);

            Enumerable.Range(0, 10)
                .AsParallel()
                .ForAll(x =>
                {
                    using (var connection = factory.CreateConnection())
                    using (var channel = connection.CreateModel())
                    {
                        for (var i = 0; i < 100 * 1000; i++)
                        {
                            var properties = channel.CreateBasicProperties();
                            var collectionId = rnd.Next(1, 1).ToString();
                            var message = new IngestEvent(collectionId, "DoStuff");

                            channel.BasicPublish("events", "", properties, serializer.Serialize(message));
                        }
                    }
                });

            Log.Information("Messages published...");
        }
    }
}