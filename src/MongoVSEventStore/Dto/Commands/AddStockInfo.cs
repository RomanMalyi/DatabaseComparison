namespace MongoVSEventStore.Dto.Commands
{
    public class AddStockInfo
    {
        public string Symbol { get; set; } = null!;
        public DateTimeOffset Date { get; set; }
        public decimal Price { get; set; }
        public decimal Position { get; set; }
    }
}
