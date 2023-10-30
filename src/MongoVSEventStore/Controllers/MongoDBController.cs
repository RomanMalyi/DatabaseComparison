using DatabaseComparison.Domain;
using DatabaseComparison.Domain.Events;
using DatabaseComparison.Dto.Commands;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace DatabaseComparison.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MongoDbController : ControllerBase
    {
        [HttpPost("currency")]
        public async Task<IActionResult> AddStock([FromRoute] Guid userId, [FromBody]AddCurrencyInfoCommand command)
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("testDatabase");
            var collection = database.GetCollection<IStoredEvent>("events");
            //TODO: separate stream for each user
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

            await collection.InsertOneAsync(@event);

            return Ok();
        }
    }
}