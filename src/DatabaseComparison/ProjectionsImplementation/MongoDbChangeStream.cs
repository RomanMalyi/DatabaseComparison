using DatabaseComparison.Domain.Events;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Newtonsoft.Json;

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
        
        while (enumerator.MoveNext())
        {
            IEnumerable<ChangeStreamDocument<BsonDocument>> changes = enumerator.Current;
            foreach(ChangeStreamDocument<BsonDocument> change in changes)
            {
                Commit commit = BsonSerializer.Deserialize<Commit>(change.FullDocument);
                var e = JsonConvert.DeserializeObject<CurrencyInfoAdded>(commit.Events[0].Payload.Body);;
                MemoryCollection.AddMongoDbEvent(e);
            }  
        }
    }

    [BsonIgnoreExtraElements]
    public class Commit
    {
        public int _id { get; set; }

        public Event[] Events { get; set; }
    }
    [BsonIgnoreExtraElements]
    public class Event
    {
        public Payload Payload { get; set; }
    }
    [BsonIgnoreExtraElements]
    public class Payload
    {
        public string Body { get; set; }
    }
}    