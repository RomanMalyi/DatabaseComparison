﻿using DatabaseComparison.Dto.Commands;
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
        private static List<AddCurrencyInfoCommand> _currencyData;
        private static int _counter = 0;

        public static void RunTest()
        {
            _currencyData = ReadFileCurrencyData();
            //this is for concurrent number of scenarios. Set numberOfScenarioInstances to 0 if you don't want it
            var testDurationSeconds = 30;
            var numberOfScenarioInstances = 1;

            //set injectionDurationSeconds to 0 if you don't want it
            var injectionDurationSeconds = 1;
            var injectionRate = 1;
            var injectionIntervalSeconds = 1;



            using var httpClient = new HttpClient();
            ScenarioProps mongoDbScenario = CreateScenario("MongoDb", httpClient, testDurationSeconds, numberOfScenarioInstances,
                injectionRate, injectionIntervalSeconds, injectionDurationSeconds);
            ScenarioProps eventStoreDbScenario = CreateScenario("EventStoreDb", httpClient, testDurationSeconds, numberOfScenarioInstances,
                injectionRate, injectionIntervalSeconds, injectionDurationSeconds);
            ScenarioProps postgreSqlDbScenario = CreateScenario("PostgreSqlDb", httpClient, testDurationSeconds, numberOfScenarioInstances,
                injectionRate, injectionIntervalSeconds, injectionDurationSeconds);

            NBomberRunner
                .RegisterScenarios(mongoDbScenario, eventStoreDbScenario, postgreSqlDbScenario)
                .WithWorkerPlugins(new HttpMetricsPlugin(new[] { HttpVersion.Version1 }))
                .Run();
        }

        private static ScenarioProps CreateScenario(string name, HttpClient httpClient, int testDurationSeconds, int numberOfScenarioInstances,
            int injectionRate, int injectionIntervalSeconds, int injectionDurationSeconds)
        {
            ScenarioProps scenario = Scenario.Create($"{name}Scenario", async context =>
                {
                    var request = Http.CreateRequest("POST", $"http://localhost:5093/api/{name}/currency")
                        .WithHeader("Accept", "application/json")
                        .WithBody(new StringContent(JsonConvert.SerializeObject(CreateCommand()), Encoding.UTF8, "application/json"));

                    var response = await Http.Send(httpClient, request);

                    return response;
                })
                .WithInit(async context => { await Task.Delay(TimeSpan.FromSeconds(1)); })
                .WithoutWarmUp()
                .WithLoadSimulations(
                    Simulation.KeepConstant(numberOfScenarioInstances, TimeSpan.FromSeconds(testDurationSeconds)),
                    Simulation.Inject(injectionRate, TimeSpan.FromSeconds(injectionIntervalSeconds),
                        TimeSpan.FromSeconds(injectionDurationSeconds)));

            return scenario;
        }

        private static AddCurrencyInfoCommand CreateCommand()
        {
            _counter++;

            if (_counter < _currencyData.Count - 1)
            {
                return _currencyData[_counter - 1];
            }
            _counter = 0;
            return _currencyData[_counter];
        }

        private static List<AddCurrencyInfoCommand> ReadFileCurrencyData()
        {
            List<AddCurrencyInfoCommand> currencyData = new List<AddCurrencyInfoCommand>();

            // Read the lines from the text file
            string[] lines = File.ReadAllLines("/Users/romanmalyi/Documents/Repositories/DatabaseComparison/src/MACD.csv");

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
