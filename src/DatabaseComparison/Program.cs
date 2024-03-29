using DatabaseComparison.DataAccess;
using DatabaseComparison.ProjectionsImplementation;
using Marten;
using Marten.Events.Daemon.Resiliency;
using Marten.Events.Projections;
using NEventStore;
using NEventStore.Serialization;
using Weasel.Core;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddEventStoreClient(builder.Configuration
    .GetSection("EventStore")
    .Get<string>() ?? string.Empty);
builder.Services.AddScoped<UserEventStore>();
builder.Services.AddSingleton<MongoStreamWrapper>();
builder.Services.AddSingleton(Wireup.Init()
    .UsingMongoPersistence("mongodb://localhost:27017/currency", new DocumentObjectSerializer())
    .InitializeStorageEngine()
    .Build());

builder.Services.AddMarten(options =>
{
    // Establish the connection string to your Marten database
    options.Connection(builder.Configuration.GetConnectionString("PostgreSql")!);
    
    options.Projections.Add(new MartenEventProjection(), ProjectionLifecycle.Async);

    // If we're running in development mode, let Marten just take care
    // of all necessary schema building and patching behind the scenes
    if (builder.Environment.IsDevelopment())
    {
        options.AutoCreateSchemaObjects = AutoCreate.All;
    }
})
.UseLightweightSessions()
.AddAsyncDaemon(DaemonMode.Solo);;

builder.Services.AddHostedService(serviceProvider => new SubscribingHostedService(serviceProvider));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
