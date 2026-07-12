using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Behaviors;

public static class BehaviorExtensions
{
    public static IServiceCollection AddValidationBehavior(this IServiceCollection services)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        return services;
    }
}
