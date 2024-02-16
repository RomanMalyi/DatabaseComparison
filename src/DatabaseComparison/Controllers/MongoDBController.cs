using DatabaseComparison.DataAccess;
using DatabaseComparison.Domain.Events;
using DatabaseComparison.Dto.Commands;
using Microsoft.AspNetCore.Mvc;

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
                ApiCallTime = DateTime.Now
            };

            wrapper.AddEvent(@event);

            return Ok();
        }
    }
}