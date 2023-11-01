using DatabaseComparison.Dto.Commands;
using NBomber.Contracts;
using NBomber.CSharp;
using NBomber.Http;
using NBomber.Http.CSharp;
using Newtonsoft.Json;
using System.Globalization;
using System.Text;

namespace NBomberLoadSimulation
{
    public static class StockLoadSimulation
    {
        private static List<AddCurrencyInfoCommand> CurrencyData;
        private static int counter = 0;

        public static void RunTest()
        {
            CurrencyData = ReadFileCurrencyData();
            //this is for concurrent number of scenarios. Set numberOfScenarioInstances to 0 if you don't want it
            var testDurationSeconds = 1;
            var numberOfScenarioInstances = 0;

            //set injectionDurationSeconds to 0 if you don't want it
            var injectionDurationSeconds = 60;
            var injectionRate = 5;
            var injectionIntervalSeconds = 1;



            using var httpClient = new HttpClient();
            ScenarioProps mongoDbScenario = CreateMongoDbScenario(httpClient, testDurationSeconds, numberOfScenarioInstances,
                injectionRate, injectionIntervalSeconds, injectionDurationSeconds);
            ScenarioProps eventStoreDbScenario = CreateEventStoreDbScenario(httpClient, testDurationSeconds, numberOfScenarioInstances,
                injectionRate, injectionIntervalSeconds, injectionDurationSeconds);
            //ScenarioProps postgreSqlDbScenario = CreatePostgreSqlDbScenario(httpClient);

            NBomberRunner
                //.RegisterScenarios(mongoDbScenario, eventStoreDbScenario, postgreSqlDbScenario)
                .RegisterScenarios(mongoDbScenario, eventStoreDbScenario)
                .WithWorkerPlugins(new HttpMetricsPlugin(new[] { HttpVersion.Version1 }))
                .Run();
        }

        private static ScenarioProps CreateMongoDbScenario(HttpClient httpClient, int testDurationSeconds, int numberOfScenarioInstances,
            int injectionRate, int injectionIntervalSeconds, int injectionDurationSeconds)
        {
            ScenarioProps scenario = Scenario.Create("MongoDbScenario", async context =>
                {
                    var request = Http.CreateRequest("POST", "https://localhost:7091/api/MongoDb/currency")
                        .WithHeader("Accept", "application/json")
                        .WithBody(new StringContent(JsonConvert.SerializeObject(CreateCommand()), Encoding.UTF8, "application/json"));

                    var response = await Http.Send(httpClient, request);

                    return response;
                })
                .WithInit(async context => { await Task.Delay(TimeSpan.FromSeconds(10)); })
                .WithoutWarmUp()
                .WithLoadSimulations(
                    Simulation.KeepConstant(numberOfScenarioInstances, TimeSpan.FromSeconds(testDurationSeconds)),
                    Simulation.Inject(injectionRate, TimeSpan.FromSeconds(injectionIntervalSeconds),
                        TimeSpan.FromSeconds(injectionDurationSeconds)));

            return scenario;
        }

        private static ScenarioProps CreateEventStoreDbScenario(HttpClient httpClient, int testDurationSeconds, int numberOfScenarioInstances,
            int injectionRate, int injectionIntervalSeconds, int injectionDurationSeconds)
        {
            ScenarioProps scenario = Scenario.Create("EventStoreDbScenario", async context =>
                {
                    var request = Http.CreateRequest("POST", "https://localhost:7091/api/EventStoreDb/currency")
                        .WithHeader("Accept", "application/json")
                        .WithBody(new StringContent(JsonConvert.SerializeObject(CreateCommand()), Encoding.UTF8, "application/json"));

                    var response = await Http.Send(httpClient, request);

                    return response;
                })
                .WithInit(async context => { await Task.Delay(TimeSpan.FromSeconds(10));})
                .WithoutWarmUp()
                .WithLoadSimulations(
                    Simulation.KeepConstant(numberOfScenarioInstances, TimeSpan.FromSeconds(testDurationSeconds)),
                    Simulation.Inject(injectionRate, TimeSpan.FromSeconds(injectionIntervalSeconds),
                        TimeSpan.FromSeconds(injectionDurationSeconds)));

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
                .WithInit(async context => { await Task.Delay(TimeSpan.FromSeconds(10)); })
                .WithoutWarmUp()
                .WithLoadSimulations(
                    Simulation.RampingInject(5, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(100)));

            return scenario;
        }

        private static AddCurrencyInfoCommand CreateCommand()
        {
            counter++;

            if (counter < CurrencyData.Count - 1)
            {
                return CurrencyData[counter - 1];
            }
            counter = 0;
            return CurrencyData[counter];
        }

        private static List<AddCurrencyInfoCommand> ReadFileCurrencyData()
        {
            List<AddCurrencyInfoCommand> currencyData = new List<AddCurrencyInfoCommand>();

            // Read the lines from the text file
            string[] lines = File.ReadAllLines("C:\\Personal\\PhD\\MACD.csv");

            // Skip the header line (assumed to be the first line)
            for (int i = 1; i < lines.Length; i++)
            {
                string[] values = lines[i].Split('\t');
                if (values.Length == 7)
                {
                    AddCurrencyInfoCommand tradingData = new AddCurrencyInfoCommand
                    {
                        Time = DateTime.ParseExact(values[0], "yyyy.MM.dd HH:mm:ss", CultureInfo.InvariantCulture),
                        TickVolume = int.Parse(values[1]),
                        RealVolume = int.Parse(values[2]),
                        High = decimal.Parse(values[3], CultureInfo.InvariantCulture),
                        Low = decimal.Parse(values[4], CultureInfo.InvariantCulture),
                        Open = decimal.Parse(values[5], CultureInfo.InvariantCulture),
                        Close = decimal.Parse(values[6], CultureInfo.InvariantCulture)
                    };

                    currencyData.Add(tradingData);
                }
            }

            return currencyData;
        }
    }
}
