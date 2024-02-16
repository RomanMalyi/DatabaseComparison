using DatabaseComparison.DataAccess;
using DatabaseComparison.Domain.Events;
using DatabaseComparison.Dto.Commands;
using Microsoft.AspNetCore.Mvc;

namespace DatabaseComparison.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventStoreDbController : ControllerBase
    {
        private readonly UserEventStore userEventStore;

        public EventStoreDbController(UserEventStore userEventStore)
        {
            this.userEventStore = userEventStore;
        }

        [HttpPost("currency")]
        public async Task<IActionResult> AddStock([FromBody] AddCurrencyInfoCommand command)
        {
            var streamName = "USD/EUR";
            var @event = new CurrencyInfoAdded
            {
                Time = command.Time,
                TickVolume = command.TickVolume,
                RealVolume = command.RealVolume,
                High = command.High,
                Low = command.Low,
                Open = command.Open,
                Close = command.Close,
                ApiCallTime = DateTime.Now
            };
            await userEventStore.AppendToStream(@event, streamName);

            return Ok();
        }
    }
}
