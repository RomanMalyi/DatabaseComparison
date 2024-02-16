using DatabaseComparison.Domain.Events;
using Marten;
using Marten.Events.Projections;

namespace DatabaseComparison.ProjectionsImplementation;

public class MartenEventProjection : EventProjection
{
    public async Task Project(CurrencyInfoAdded e, IDocumentOperations ops)
    {
        MemoryCollection.AddPostgreSqlEvent(e);
    }
}