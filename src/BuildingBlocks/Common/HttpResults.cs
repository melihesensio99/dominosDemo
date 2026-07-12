using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Common;

public static class HttpResults
{
    public static IResult ToHttpResult(this Result result)
    {
        if (result.IsSuccess)
        {
            return Results.Ok();
        }

        return CreateErrorResult(result.Error);
    }

    public static IResult ToHttpResult<T>(this Result<T> result)
    {
        if (result.IsSuccess && result.Value is not null)
        {
            return Results.Ok(result.Value);
        }

        return CreateErrorResult(result.Error);
    }

    private static IResult CreateErrorResult(Error? error)
    {
        var statusCode = error?.Type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status400BadRequest,
        };

        return Results.Json(new
        {
            error = error?.Code ?? "unknown_error",
            message = error?.Message ?? "Something went wrong.",
        }, statusCode: statusCode);
    }
}
