using MongoDB.Bson;
using MongoDB.Driver;

namespace DatabaseComparison.ProjectionsImplementation;

public class MongoDbChangeStream
{
    private readonly IMongoCollection<BsonDocument> collection;

    public MongoDbChangeStream()
    {
        //TODO: use appsettings connection string
        collection = new MongoClient("")
            .GetDatabase("currency")
            .GetCollection<BsonDocument>("Commits");
    }

    public async Task WatchCollectionAsync(CancellationToken cancellationToken)
    {
        var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<BsonDocument>>()
            .Match(change => change.OperationType == ChangeStreamOperationType.Insert);
        
        ChangeStreamOptions options = new ()
        {
            FullDocument = ChangeStreamFullDocumentOption.UpdateLookup
        };
        
        using IChangeStreamCursor<ChangeStreamDocument<BsonDocument>> enumerator = collection.Watch(
            pipeline, 
            options
        );

        Console.WriteLine("Waiting for changes...");
        while (enumerator.MoveNext())
        {
            IEnumerable<ChangeStreamDocument<BsonDocument>> changes = enumerator.Current;
            foreach(ChangeStreamDocument<BsonDocument> change in changes)
            {
                Console.WriteLine(change);
            }  
        }
    }
}