using BuildingBlocks.Common;

namespace Auth.Api.Common;

public static class HttpResults
{
    public static IResult ToHttpResult<T>(this Result<T> result)
    {
        if (result.IsSuccess && result.Value is not null)
        {
            return Results.Ok(result.Value);
        }

        return Results.BadRequest(new
        {
            error = result.Error?.Code ?? "unknown_error",
            message = result.Error?.Message ?? "Something went wrong.",
        });
    }
}
