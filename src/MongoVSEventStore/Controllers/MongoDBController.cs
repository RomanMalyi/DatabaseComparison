using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoVSEventStore.Domain;
using MongoVSEventStore.Domain.Events;
using MongoVSEventStore.Dto.Commands;

namespace MongoVSEventStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MongoDbController : ControllerBase
    {
        [HttpPost("users")]
        public async Task<IActionResult> CreateUser(CreateUser command)
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("testDatabase");
            var collection = database.GetCollection<IStoredEvent>("events");
            var document = new UserCreated
            {
                CreatedAt = DateTimeOffset.Now, Id = command.Id, Name = command.Name
            };

            await collection.InsertOneAsync(document);

            return Ok();
        }

        [HttpPost("users/{userId}/stock")]
        public async Task<IActionResult> AddStock([FromRoute] Guid userId, [FromBody]AddStockInfo command)
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("testDatabase");
            var collection = database.GetCollection<IStoredEvent>("events");
            //TODO: separate stream for each user
            var document = new StockInfoAdded
            {
                Symbol = command.Symbol,
                Date = command.Date,
                Position = command.Position,
                Price = command.Price
            };

            await collection.InsertOneAsync(document);

            return Ok();
        }
    }
}