using EventStore.Client;

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
                    Console.WriteLine($"Received event {evnt.OriginalEventNumber}@{evnt.OriginalStreamId}");
                    //await HandleEvent(evnt);
                }, cancellationToken: stoppingToken);

            var mongoChangeStream = new MongoDbChangeStream();
            await mongoChangeStream.WatchCollectionAsync(stoppingToken);
        }
    }
}