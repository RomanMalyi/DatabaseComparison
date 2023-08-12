namespace MongoVSEventStore.Domain.Events
{
    public class StockInfoAdded : IStoredEvent
    {
        public string Symbol { get; set; } = null!;
        public DateTimeOffset Date { get; set; }
        public decimal Price { get; set; }
        public decimal Position { get; set; }
    }
}
