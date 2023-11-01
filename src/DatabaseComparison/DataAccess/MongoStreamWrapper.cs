using DatabaseComparison.Domain.Events;
using NEventStore.Serialization;
using NEventStore;

namespace DatabaseComparison.DataAccess
{
    public class MongoStreamWrapper
    {
        private readonly object executionLock = new object();

        public void AddEvent(CurrencyInfoAdded @event)
        {
            lock (executionLock)
            {
                var store = Wireup.Init()
                    .UsingMongoPersistence("mongodb://localhost:27017/testDatabase", new DocumentObjectSerializer())
                    .InitializeStorageEngine()
                    .Build();
                using (store)
                {
                    using (var stream = store.OpenStream("USD/EUR"))//TODO: first time you need to use CREATESTREAM
                    {
                        stream.Add(new EventMessage { Body = Newtonsoft.Json.JsonConvert.SerializeObject(@event) });
                        stream.CommitChanges(Guid.NewGuid());
                    }
                }
            }
        }
    }
}
