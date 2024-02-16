using DatabaseComparison.Domain.Events;
using DatabaseComparison.Dto.Commands;
using Marten;
using Microsoft.AspNetCore.Mvc;

namespace DatabaseComparison.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostgreSqlDbController : ControllerBase
    {
        private readonly Guid streamId = new Guid("21633f3c-ef78-48e3-969b-3c5c7295b12a");//Instead of "USD/EUR"
        private readonly IDocumentSession session;

        public PostgreSqlDbController(IDocumentSession session)
        {
            this.session = session;
        }

        //pgAdmin ->PGADMIN_DEFAULT_EMAIL=myemail@example.com + PGADMIN_DEFAULT_PASSWORD=SuperSecret

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

            try
            {
                await session.Events.AppendExclusive(streamId, @event);
                await session.SaveChangesAsync();
            }
            catch (Exception e)
            {//first time use StartStream
                session.Events.StartStream(streamId, @event);
                await session.SaveChangesAsync();
            }

            return Ok();
        }
    }
}
