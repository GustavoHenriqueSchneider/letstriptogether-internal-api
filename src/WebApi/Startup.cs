using LetsTripTogether.InternalApi.Application;
using LetsTripTogether.InternalApi.Infrastructure;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace LetsTripTogether.InternalApi.WebApi;

public class Startup(IConfiguration configuration, IWebHostEnvironment environment)
{
    public void ConfigureServices(IServiceCollection services)
    {
        // Application Layer
        services.RegisterApplicationUseCases();
        
        // Infrastructure Layer
        services.RegisterApplicationConfigurations(configuration);
        services.RegisterApplicationAuthentication(configuration);
        services.RegisterApplicationAuthorization();
        services.RegisterApplicationExternalDependencies(configuration);
        
        // WebApi Layer
        services.RegisterApplicationApiServices();
        
        // Health Checks
        services.AddHealthChecks();
    }

    public void Configure(
        IApplicationBuilder app, 
        IWebHostEnvironment env, 
        IApiVersionDescriptionProvider apiVersionDescriptionProvider)
    {
        // Error Handling
        if (environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/api/error");
            app.UseHsts();
        }

        // Swagger - dispon?vel conforme configura??o (padr?o: habilitado)
        var swaggerEnabled = configuration.GetValue<bool>("Swagger:Enabled", true);
        if (swaggerEnabled)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                // Configura o Swagger UI para mostrar todas as vers?es da API
                foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint(
                        $"/swagger/{description.GroupName}/swagger.json",
                        $"LetsTripTogether API {description.GroupName.ToUpperInvariant()}");
                }
                
                options.RoutePrefix = string.Empty; // Swagger UI na raiz
                options.DocumentTitle = "LetsTripTogether Internal API";
            });
        }

        // Security & Routing
        app.UseHttpsRedirection();
        app.UseRouting();
        
        // Authentication & Authorization
        app.UseAuthentication();
        app.UseAuthorization();
        
        // Health Checks
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHealthChecks("/health");
        });
    }
}