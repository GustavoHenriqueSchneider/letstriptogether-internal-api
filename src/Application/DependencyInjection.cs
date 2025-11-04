using System.Reflection;
using FluentValidation;
using LetsTripTogether.InternalApi.Application.Common.Behaviours;
using LetsTripTogether.InternalApi.Application.Common.Extensions;
using LetsTripTogether.InternalApi.Application.Common.Interfaces.Extensions;
using MediatR;
using Microsoft.AspNetCore.Http;
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

            configuration.AddOpenBehavior(typeof(ValidationBehaviour<,>));
            configuration.AddOpenBehavior(typeof(UnhandledExceptionBehaviour<,>));
            mediator?.Invoke(configuration);
        });

        services.AddHttpContextAccessor();
        services.AddTransient<IHttpContextExtensions, HttpContextExtensions>();
        
        services.AddScoped<IApplicationUserContextExtensions>(provider =>
        {
            var httpContextAccessor = provider.GetRequiredService<IHttpContextAccessor>();
            var principal = httpContextAccessor.HttpContext?.User 
                ?? throw new InvalidOperationException("HttpContext or User is not available");
            return new ApplicationUserContextExtensions(principal);
        });
    }
}