using Confluent.Kafka;
using DatabaseComparison.Domain.Events;
using DatabaseComparison.Dto.Commands;
using Microsoft.AspNetCore.Mvc;

namespace DatabaseComparison.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KafkaController : ControllerBase
    {
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
            };

            var config = KafkaProducerConfig.GetConfig();

            using var producer = new ProducerBuilder<Null, string>(config).Build();
            string topic = "firstOne"; // Replace with your topic name
            string message = Newtonsoft.Json.JsonConvert.SerializeObject(@event);
            var deliveryReport = await producer.ProduceAsync(topic, new Message<Null, string> { Value = message });

            //await userEventStore.AppendToStream(@event, streamName);

            return Ok();
        }

        public static class KafkaProducerConfig
        {
            public static ProducerConfig GetConfig()
            {
                return new ProducerConfig
                {
                    BootstrapServers = "localhost:29092", // Replace with your Kafka broker address
                    ClientId = "KafkaExampleProducer",
                };
            }
        }
    }
}
