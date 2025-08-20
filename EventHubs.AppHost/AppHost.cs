var builder = DistributedApplication.CreateBuilder(args);

var serviceBus = builder.AddAzureServiceBus("servicebus")
    .RunAsEmulator();

var eventHub = builder.AddAzureEventHubs("eventhubns")
    .RunAsEmulator();
var checkpoint = builder.AddAzureStorage("checkpoint")
    .RunAsEmulator()
    .AddBlobs("checkpoints");
var demoEvent = eventHub.AddHub("hub");
var demoEventConsumer = demoEvent.AddConsumerGroup("hub-consumer-group");

var apiService = builder.AddProject<Projects.EventHubs_Api>("apiservice")
    .WithHttpHealthCheck("/health")
    .WithReference(serviceBus)
    .WithReference(eventHub)
    .WithReference(demoEvent)
    .WithReference(demoEventConsumer)
    .WithReference(checkpoint)
    .WaitFor(serviceBus);

builder.AddProject<Projects.EventHubs_Consumer>("consumer")
    .WithReference(serviceBus)
    .WithReference(eventHub)
    .WithReference(checkpoint)
    .WithReference(demoEvent)
    .WaitFor(apiService);

await builder.Build().RunAsync();
