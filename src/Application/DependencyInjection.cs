using System.Reflection;
using FluentValidation;
using LetsTripTogether.InternalApi.Application.Common.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace LetsTripTogether.InternalApi.Application;

public static class DependencyInjection
{
    public static void RegisterApplicationUseCases(this IServiceCollection services)
    {
        services.AddRegisterApplicationBehaviours(Assembly.GetExecutingAssembly());
    }

    private static void AddRegisterApplicationBehaviours(this IServiceCollection services,
        Assembly assembly, Action<MediatRServiceConfiguration>? mediator = null)
    {
        services.AddAutoMapper(assembly);
        services.AddValidatorsFromAssembly(assembly);
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(assembly);

            //configuration.AddOpenBehavior(typeof(UnhandledExceptionBehaviour<,>));
            //configuration.AddOpenBehavior(typeof(ValidationBehaviour<,>));
            mediator?.Invoke(configuration);
        });

        services.AddHttpContextAccessor();
        services.AddTransient<HttpContextExtensions>();
    }
}