using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using PriceTrackerCloud.Application.Behaviors;
using PriceTrackerCloud.Application.Mappings;

namespace PriceTrackerCloud.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // MediatR escanea el ensamblado y registra todos los handlers automáticamente
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        // Pipeline: cada request pasa por ValidationBehavior antes de llegar al handler
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        // AutoMapper escanea el ensamblado buscando clases que hereden de Profile
        services.AddAutoMapper(typeof(MappingProfile).Assembly);

        // FluentValidation registra todos los validators del ensamblado
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        return services;
    }
}
