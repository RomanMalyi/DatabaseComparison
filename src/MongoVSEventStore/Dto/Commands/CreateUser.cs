namespace MongoVSEventStore.Dto.Commands
{
    public class CreateUser
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
    }
}
