namespace Gateway.Api.Options;

public sealed class GatewayOptions
{
    public const string SectionName = "DownstreamServices";

    public Dictionary<string, string> DownstreamServices { get; init; } = new(StringComparer.OrdinalIgnoreCase);
}
