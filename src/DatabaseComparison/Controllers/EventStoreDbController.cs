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
        public async Task<IActionResult> AddStock([FromRoute] Guid userId, [FromBody] AddCurrencyInfoCommand command)
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
            await userEventStore.AppendToStream(@event, userId.ToString());

            return Ok();
        }
    }
}
