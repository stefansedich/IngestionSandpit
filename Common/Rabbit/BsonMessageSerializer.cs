using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace Common.Rabbit
{
    public class BsonMessageSerializer : IMessageSerializer
    {
        private readonly JsonSerializer _serializer;

        public BsonMessageSerializer()
        {
            _serializer = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                ObjectCreationHandling = ObjectCreationHandling.Auto,
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                TypeNameHandling = TypeNameHandling.All,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            };
        }

        public byte[] Serialize(object message)
        {
            using (var ms = new MemoryStream())
            using (var writer = new BsonWriter(ms))
            {
                _serializer.Serialize(writer, message);

                return ms.ToArray();
            }
        }

        public object Deserialize(byte[] data)
        {
            using (var ms = new MemoryStream(data))
            using (var reader = new BsonReader(ms))
            {
                return _serializer.Deserialize(reader);
            }
        }
    }
}