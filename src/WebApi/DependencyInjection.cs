using FluentValidation.AspNetCore;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;

namespace LetsTripTogether.InternalApi.WebApi;

public static class DependencyInjection
{
    public static void RegisterApplicationApiServices(this IServiceCollection services)
    {
        services
            .AddEndpointsApiExplorer()
            .AddRouting(options => options.LowercaseUrls = true);

        services
            .AddFluentValidationClientsideAdapters()
            .AddFluentValidationAutoValidation();
        
        services.AddControllers();

        services
            .AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            })
            .AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });
        
        services
            .AddSwaggerGen()
            .AddFluentValidationRulesToSwagger();
    }
}