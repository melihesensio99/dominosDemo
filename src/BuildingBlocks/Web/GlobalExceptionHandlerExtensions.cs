using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BuildingBlocks.Web;

public static class GlobalExceptionHandlerExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(exceptionApp =>
        {
            exceptionApp.Run(async context =>
            {
                var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
                if (exception is null)
                {
                    return;
                }

                context.Response.ContentType = "application/problem+json";

                switch (exception)
                {
                    case ValidationException validationException:
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;

                        var validationErrors = validationException.Errors
                            .GroupBy(error => error.PropertyName)
                            .ToDictionary(
                                group => string.IsNullOrWhiteSpace(group.Key) ? "validation" : group.Key,
                                group => group.Select(error => error.ErrorMessage).ToArray());

                        await context.Response.WriteAsJsonAsync(new ValidationProblemDetails(validationErrors)
                        {
                            Status = StatusCodes.Status400BadRequest,
                            Title = "Validation failed",
                        });
                        break;

                    default:
                        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                        await context.Response.WriteAsJsonAsync(new ProblemDetails
                        {
                            Status = StatusCodes.Status500InternalServerError,
                            Title = "Unexpected error",
                            Detail = "An unexpected error occurred.",
                        });
                        break;
                }
            });
        });

        return app;
    }
}
