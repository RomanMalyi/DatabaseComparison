using Microsoft.AspNetCore.Mvc;
using MongoVSEventStore.DataAccess;
using MongoVSEventStore.Domain.Events;
using MongoVSEventStore.Dto.Commands;

namespace MongoVSEventStore.Controllers
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

        [HttpPost("users")]
        public async Task<IActionResult> Post(CreateUser command)
        {
            var @event = new UserCreated() { Id = command.Id, CreatedAt = DateTimeOffset.Now, Name = command.Name};
            await userEventStore.AppendToStream(@event, @event.Id.ToString());

            return Ok();
        }

        [HttpPost("users/{userId}/stock")]
        public async Task<IActionResult> AddStock([FromRoute] Guid userId, [FromBody] AddStockInfo command)
        {
            var @event = new StockInfoAdded
            {
                Symbol = command.Symbol,
                Date = command.Date,
                Position = command.Position,
                Price = command.Price
            };
            await userEventStore.AppendToStream(@event, userId.ToString());

            return Ok();
        }
    }
}
