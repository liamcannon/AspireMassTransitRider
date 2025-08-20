namespace EventHubs.ServiceDefaults.Contracts;

public record EventHubMessage
{
    public string Text { get; set; } = null!;
}
