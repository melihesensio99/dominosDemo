namespace Catalog.Api.Application.Common;

public static class SlugHelper
{
    public static string Slugify(string value) =>
        value.Trim().ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("_", "-");
}
