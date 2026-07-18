namespace Gateway.Api.Options;

public sealed class GatewayOptions
{
    public const string SectionName = "ServiceMap";

    public Dictionary<string, string> Services { get; init; } = new(StringComparer.OrdinalIgnoreCase);
}
