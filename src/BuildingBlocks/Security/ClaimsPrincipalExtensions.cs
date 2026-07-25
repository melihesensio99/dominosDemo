using System.Security.Claims;

namespace BuildingBlocks.Security;

public static class ClaimsPrincipalExtensions
{
    public static bool TryGetUserId(this ClaimsPrincipal user, out string userId)
    {
        userId = user.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? user.FindFirstValue("sub")
            ?? string.Empty;

        return !string.IsNullOrWhiteSpace(userId);
    }
}
