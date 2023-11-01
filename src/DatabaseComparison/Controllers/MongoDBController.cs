using Amazon.Runtime.EventStreams.Internal;
using DatabaseComparison.Domain.Events;
using DatabaseComparison.Dto.Commands;
using Microsoft.AspNetCore.Mvc;
using NEventStore;
using NEventStore.Serialization;

namespace DatabaseComparison.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MongoDbController : ControllerBase
    {
        private readonly MongoStreamWrapper wrapper;

        public MongoDbController(MongoStreamWrapper wrapper)
        {
            this.wrapper = wrapper;
        }

        [HttpPost("currency")]
        public async Task<IActionResult> AddStock([FromBody] AddCurrencyInfoCommand command)
        {
            var @event = new CurrencyInfoAdded
            {
                Time = command.Time,
                TickVolume = command.TickVolume,
                RealVolume = command.RealVolume,
                High = command.High,
                Low = command.Low,
                Open = command.Open,
                Close = command.Close,
            };

            wrapper.AddEvent(@event);

            return Ok();
        }
    }

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