using LetsTripTogether.InternalApi.Application;
using LetsTripTogether.InternalApi.Infrastructure;

namespace LetsTripTogether.InternalApi.WebApi;

public class Startup(IConfiguration configuration, IWebHostEnvironment environment)
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.RegisterApplicationUseCases();
        services.RegisterApplicationConfigurations(configuration);
        services.RegisterApplicationAuthentication(configuration);
        services.RegisterApplicationAuthorization();
        services.RegisterApplicationExternalDependencies(configuration);
        services.RegisterApplicationApiServices();
        services.AddHealthChecks();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthentication();
        app.UseAuthorization();
        app.UseHttpsRedirection();
        app.UseEndpoints(endpoints => endpoints.MapControllers());
    }
}