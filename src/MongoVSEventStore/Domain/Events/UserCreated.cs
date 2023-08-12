namespace MongoVSEventStore.Domain.Events
{
    public class UserCreated : IStoredEvent
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public DateTimeOffset CreatedAt { get; set; }
    }
}
