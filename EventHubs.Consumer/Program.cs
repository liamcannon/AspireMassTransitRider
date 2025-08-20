using EventHubs.Consumer;
using MassTransit;
using EventHubs.ServiceDefaults.Contracts;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.AddServiceDefaults();

builder.Services.AddMassTransit(x =>
{
    x.UsingAzureServiceBus((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("serviceBus"));
        cfg.ConfigureEndpoints(ctx);
    });

    x.AddRider(rider =>
    {
        rider.AddConsumer<EventHubMessageConsumer>();

        rider.UsingEventHub((context, k) =>
        {
            k.Host(builder.Configuration.GetConnectionString("hub"));

            k.Storage(builder.Configuration.GetConnectionString("checkpoints"));

            k.ReceiveEndpoint("hub", "hub-consumer-group", c =>
            {
                c.ConfigureConsumer<EventHubMessageConsumer>(context);
            });
        });
    });
});

var host = builder.Build();
await host.RunAsync();


class EventHubMessageConsumer :
        IConsumer<EventHubMessage>
{
    public Task Consume(ConsumeContext<EventHubMessage> context)
    {
        return Task.CompletedTask;
    }
}