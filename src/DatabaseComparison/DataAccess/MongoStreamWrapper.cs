using DatabaseComparison.Domain.Events;
using NEventStore;

namespace DatabaseComparison.DataAccess
{
    public class MongoStreamWrapper
    {
        private readonly IStoreEvents storeEvents;
        public MongoStreamWrapper(IStoreEvents storeEvents)
        {
            this.storeEvents = storeEvents;
        }

        public void AddEvent(CurrencyInfoAdded @event)
        {
            using (storeEvents)
            {
                try
                {
                    using var stream = storeEvents.OpenStream("USD/EUR");
                    stream.Add(new EventMessage { Body = Newtonsoft.Json.JsonConvert.SerializeObject(@event) });
                    stream.CommitChanges(Guid.NewGuid());
                }
                catch (Exception e)
                {
                    using var stream = storeEvents.CreateStream("USD/EUR");
                    stream.Add(new EventMessage { Body = Newtonsoft.Json.JsonConvert.SerializeObject(@event) });
                    stream.CommitChanges(Guid.NewGuid());
                }
            }
        }
    }
}
