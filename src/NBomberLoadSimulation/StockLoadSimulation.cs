using NBomber.Contracts;
using NBomber.CSharp;
using NBomber.Http;
using NBomber.Http.CSharp;

namespace NBomberLoadSimulation
{
    public static class StockLoadSimulation
    {
        public static void RunTest()
        {
            using var httpClient = new HttpClient();
            ScenarioProps mongoDbScenario = CreateMongoDbScenario(httpClient);
            ScenarioProps eventStoreDbScenario = CreateEventStoreDbScenario(httpClient);
            ScenarioProps postgreSqlDbScenario = CreatePostgreSqlDbScenario(httpClient);

            NBomberRunner
                .RegisterScenarios(mongoDbScenario, eventStoreDbScenario, postgreSqlDbScenario)
                .WithWorkerPlugins(new HttpMetricsPlugin(new[] { HttpVersion.Version1 }))
                .Run();
        }

        private static ScenarioProps CreateMongoDbScenario(HttpClient httpClient)
        {
            ScenarioProps scenario = Scenario.Create("MongoDbScenario", async context =>
                {
                    var request = Http.CreateRequest("POST", "https://localhost:7091/api/MongoDb/stream")
                        .WithHeader("Accept", "application/json");

                    var response = await Http.Send(httpClient, request);

                    return response;
                })
                .WithInit(async context =>
                { 
                    await Task.Delay(TimeSpan.FromSeconds(10));
                })
                .WithoutWarmUp()
                .WithLoadSimulations(
                    Simulation.RampingInject(5, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(100)));

            return scenario;
        }

        private static ScenarioProps CreateEventStoreDbScenario(HttpClient httpClient)
        {
            ScenarioProps scenario = Scenario.Create("EventStoreDbScenario", async context =>
                {
                    var request = Http.CreateRequest("POST", "https://localhost:7091/api/EventStoreDb/stream")
                        .WithHeader("Accept", "application/json");

                    var response = await Http.Send(httpClient, request);

                    return response;
                })
                .WithInit(async context =>
                {
                    await Task.Delay(TimeSpan.FromSeconds(10));
                })
                .WithoutWarmUp()
                .WithLoadSimulations(
                    Simulation.RampingInject(5, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(100)));

            return scenario;
        }

        private static ScenarioProps CreatePostgreSqlDbScenario(HttpClient httpClient)
        {
            ScenarioProps scenario = Scenario.Create("PostgreSqlDbScenario", async context =>
                {
                    var request = Http.CreateRequest("POST", "https://localhost:7091/api/PostgreSqlDb/stream")
                        .WithHeader("Accept", "application/json");

                    var response = await Http.Send(httpClient, request);

                    return response;
                })
                .WithInit(async context =>
                {
                    await Task.Delay(TimeSpan.FromSeconds(10));
                })
                .WithoutWarmUp()
                .WithLoadSimulations(
                    Simulation.RampingInject(5, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(100)));

            return scenario;
        }
    }
}
