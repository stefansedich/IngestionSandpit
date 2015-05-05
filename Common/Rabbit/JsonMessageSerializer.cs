using System.Text;
using Newtonsoft.Json;

namespace Common.Rabbit
{
    public class JsonMessageSerializer : IMessageSerializer
    {
        public static JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            ObjectCreationHandling = ObjectCreationHandling.Auto,
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            TypeNameHandling = TypeNameHandling.All,
            PreserveReferencesHandling = PreserveReferencesHandling.Objects
        };

        public byte[] Serialize(object message)
        {
            var json = JsonConvert.SerializeObject(message, SerializerSettings);

            return Encoding.Default.GetBytes(json);
        }

        public object Deserialize(byte[] data)
        {
            var json = Encoding.Default.GetString(data);

            return JsonConvert.DeserializeObject(json, SerializerSettings);
        }
    }
}