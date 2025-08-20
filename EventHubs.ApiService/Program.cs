using EventHubs.ServiceDefaults.Contracts;
using MassTransit;

const string eventHubName = "hub";

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMassTransit(x =>
{
    x.UsingAzureServiceBus((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("serviceBus"));
        cfg.ConfigureEndpoints(ctx);
    });

    x.AddRider(rider =>
    {
        rider.UsingEventHub((context, k) =>
        {
            k.Host(builder.Configuration.GetConnectionString("hub"));
            k.Storage(builder.Configuration.GetConnectionString("checkpoints"));
        });
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(opts =>
    {
        opts.SwaggerEndpoint("/swagger/v1/swagger.json", "EventHubs API V1");
        opts.RoutePrefix = string.Empty; 
    });
}

app.MapPost("/event", async (EventHubMessage message, IEventHubProducerProvider provider) =>
{
    IEventHubProducer producer = await provider.GetProducer(eventHubName);
    await producer.Produce<EventHubMessage>(message);
    return Results.Accepted();
});

app.MapDefaultEndpoints();

await app.RunAsync();
