namespace Common.Rabbit
{
    public interface IMessageSerializer
    {
        byte[] Serialize(object message);
        object Deserialize(byte[] data);
    }
}