using NEventStore;
using NEventStore.Serialization;

namespace DatabaseComparison.Domain.Events
{
    [Serializable]
    public class CurrencyInfoAdded : IStoredEvent
    {
        public DateTime Time { get; set; }
        public int TickVolume { get; set; }
        public int RealVolume { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Open { get; set; }
        public decimal Close { get; set; }
    }

    public class CurrencyInfoAddedDocumentSerializer : IDocumentSerializer
    {
        public object Serialize<T>(T graph)
        {
            T a;
            //return Newtonsoft.Json.JsonConvert.SerializeObject(graph);
            return new EventMessage() { Body = Newtonsoft.Json.JsonConvert.SerializeObject(graph) };
        }

        public T Deserialize<T>(object document)
        {
            T a;
            if (document is string json)
            {
                // Use a JSON serializer to deserialize the JSON string back to the generic object.
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
            }

            return default;
        }
    }
}
