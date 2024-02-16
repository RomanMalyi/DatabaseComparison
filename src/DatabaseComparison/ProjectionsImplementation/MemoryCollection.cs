using System.Collections.Concurrent;
using DatabaseComparison.Domain.Events;

namespace DatabaseComparison.ProjectionsImplementation;

public static class MemoryCollection
{
    private static readonly ConcurrentBag<CurrencyInfoAdded> PostgreSqlEvents;
    private static readonly ConcurrentBag<CurrencyInfoAdded> MongoDbEvents;
    private static readonly ConcurrentBag<CurrencyInfoAdded> EventStoreDbEvents;
    
    static MemoryCollection()
    {
        PostgreSqlEvents = new ConcurrentBag<CurrencyInfoAdded>();
        MongoDbEvents = new ConcurrentBag<CurrencyInfoAdded>();
        EventStoreDbEvents = new ConcurrentBag<CurrencyInfoAdded>();
    }

    public static void AddPostgreSqlEvent(CurrencyInfoAdded e)
    {
        e.SaveTime = DateTime.Now;
        PostgreSqlEvents.Add(e);
    }
    
    public static void AddMongoDbEvent(CurrencyInfoAdded e)
    {
        e.SaveTime = DateTime.Now;
        MongoDbEvents.Add(e);
    }
    
    public static void AddEventStoreDbEvent(CurrencyInfoAdded e)
    {
        e.SaveTime = DateTime.Now;
        EventStoreDbEvents.Add(e);
    }

    public static AverageProjectionTime GetAverageTime()
    {
        if (PostgreSqlEvents.Count == 0 || MongoDbEvents.Count == 0 || EventStoreDbEvents.Count == 0)
            return new AverageProjectionTime();
        
        return new AverageProjectionTime()
        {
            PostgreSql = new AverageTimeInfo()
            {
                AverageMs = PostgreSqlEvents.Select(e => e.SaveTime - e.ApiCallTime).ToList().Average(t => t.Milliseconds),
                NumberOfElements = PostgreSqlEvents.Count
            },
            MongoDb = new AverageTimeInfo()
            {
                AverageMs = MongoDbEvents.Select(e => e.SaveTime - e.ApiCallTime).ToList().Average(t => t.Milliseconds),
                NumberOfElements = MongoDbEvents.Count
            },
            EventStore = new AverageTimeInfo()
            {
                AverageMs = EventStoreDbEvents.Select(e => e.SaveTime - e.ApiCallTime).ToList().Average(t => t.Milliseconds),
                NumberOfElements = EventStoreDbEvents.Count
            }
        };
    }
    
    public class AverageProjectionTime
    {
        public AverageTimeInfo PostgreSql { get; set; }
        public AverageTimeInfo MongoDb { get; set; }
        public AverageTimeInfo EventStore { get; set; }
    }
    
    public class AverageTimeInfo
    {
        public double AverageMs { get; set; }
        public int NumberOfElements { get; set; }
    }
}