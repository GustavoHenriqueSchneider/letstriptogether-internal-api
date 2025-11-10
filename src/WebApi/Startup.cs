using Application;
using Infrastructure;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace WebApi;

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

    public void Configure(
        IApplicationBuilder app, 
        IWebHostEnvironment env, 
        IApiVersionDescriptionProvider apiVersionDescriptionProvider)
    {
        app.UseExceptionHandler("/api/error");
        
        if (!environment.IsDevelopment())
        {
            app.UseHsts();
        }
        
        var swaggerEnabled = configuration.GetValue("Swagger:Enabled", true);
        if (swaggerEnabled)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint(
                        $"/swagger/{description.GroupName}/swagger.json",
                        $"LetsTripTogether API {description.GroupName.ToUpperInvariant()}");
                }
                
                options.RoutePrefix = "swagger";
                options.DocumentTitle = "LetsTripTogether Internal API";
            });
        }
        
        app.UseHttpsRedirection();
        app.UseRouting();
        
        app.UseAuthentication();
        app.UseAuthorization();
        
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHealthChecks("/health");
        });
    }
}