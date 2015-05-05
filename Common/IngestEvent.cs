namespace Common
{
    public class IngestEvent
    {
        public string CollectionId { get; set; }
        public string Type { get; set; }

        public IngestEvent()
        {    
        }
        
        public IngestEvent(string collectionId, string type)
        {
            CollectionId = collectionId;
            Type = type;
        }
    }
}