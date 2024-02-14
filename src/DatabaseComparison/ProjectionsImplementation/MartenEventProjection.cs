using DatabaseComparison.Domain;
using DatabaseComparison.Domain.Events;
using Marten;
using Marten.Events.Projections;

namespace DatabaseComparison.ProjectionsImplementation;

public class MartenEventProjection : EventProjection
{
    public async Task Project(CurrencyInfoAdded e, IDocumentOperations ops)
    {
        Console.WriteLine($"Received event high:{e.High}, volume:{e.RealVolume}");
    }
}