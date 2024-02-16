using DatabaseComparison.Domain.Events;
using EventStore.Client;
using Newtonsoft.Json;

namespace DatabaseComparison.ProjectionsImplementation;

public class SubscribingHostedService : BackgroundService
{
    private readonly IServiceProvider serviceProvider;
    
    public SubscribingHostedService(IServiceProvider sp)
    {
        serviceProvider = sp;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var client = 
                scope.ServiceProvider
                    .GetRequiredService<EventStoreClient>();
            
            //EventStoreDB events handler
            await client.SubscribeToAllAsync(
                FromAll.Start,
                async (subscription, evnt, cancellationToken) => {
                    MemoryCollection.AddEventStoreDbEvent(DeserializeFromMemory(evnt.OriginalEvent.Data));
                }, cancellationToken: stoppingToken);

            var mongoChangeStream = new MongoDbChangeStream();
            await mongoChangeStream.WatchCollectionAsync(stoppingToken);
        }
    }
    
    public static CurrencyInfoAdded DeserializeFromMemory(ReadOnlyMemory<byte> data)
    {
        // Convert ReadOnlyMemory<byte> to a string
        string json = System.Text.Encoding.UTF8.GetString(data.Span);

        // Deserialize the JSON string to CurrencyInfoAdded object
        CurrencyInfoAdded result = JsonConvert.DeserializeObject<CurrencyInfoAdded>(json);

        return result;
    }
}