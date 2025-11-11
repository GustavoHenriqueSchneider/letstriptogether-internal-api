using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Text;
using Application.Common.Interfaces.Services;
using Application.Common.Policies;
using Domain.Aggregates.DestinationAggregate;
using Domain.Aggregates.GroupAggregate;
using Domain.Aggregates.RoleAggregate;
using Domain.Aggregates.UserAggregate;
using Domain.Common;
using Domain.Security;
using Domain.ValueObjects;
using Infrastructure.Clients;
using Infrastructure.Configurations;
using Infrastructure.EntityFramework.Context;
using Infrastructure.Repositories.Destinations;
using Infrastructure.Repositories.Groups;
using Infrastructure.Repositories.Roles;
using Infrastructure.Repositories.Users;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection RegisterApplicationExternalDependencies(
        this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment? environment = null)
    {
        services.AddDbContext(configuration, environment);
        services.AddRepositories();
        
        services.AddClients(configuration);
        services.AddServices(configuration);
        
        return services;
    }
    
    public static void RegisterApplicationConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSection = configuration.GetRequiredSection(nameof(JsonWebTokenSettings));
        services.Configure<JsonWebTokenSettings>(jwtSection);
        
        var emailTemplateSection = configuration.GetSection(nameof(EmailTemplateSettings));
        services.Configure<EmailTemplateSettings>(emailTemplateSection);
    }

    public static void RegisterApplicationAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration
            .GetRequiredSection(nameof(JsonWebTokenSettings))
            .Get<JsonWebTokenSettings>()!;

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = jwtSettings.Issuer,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
                };
            });
    }

    public static void RegisterApplicationAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.DefaultPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireClaim(Claims.TokenType, TokenTypes.Access)
                .Build();
    
            options.AddPolicy(Policies.RegisterValidateEmail, policy =>
                policy.RequireClaim(Claims.TokenType, TokenTypes.Step)
                    .RequireClaim(Claims.Step, Step.ValidateEmail));

            options.AddPolicy(Policies.RegisterSetPassword, policy => 
                policy.RequireClaim(Claims.TokenType, TokenTypes.Step)
                    .RequireClaim(Claims.Step, Step.SetPassword));

            options.AddPolicy(Policies.ResetPassword, policy =>
                policy.RequireClaim(Claims.TokenType, TokenTypes.ResetPassword));

            options.AddPolicy(Policies.Admin, policy => 
                policy.RequireRole(Roles.Admin).RequireClaim(Claims.TokenType, TokenTypes.Access));
        });
        
        services.AddTransient<JwtSecurityTokenHandler>();
    }

    private static void AddDbContext(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment? environment)
    {
        var isProduction = environment?.IsProduction() ?? false;
        
        services.AddDbContext<IUnitOfWork, AppDbContext>(options =>
        {
            var postgresConnection = configuration.GetConnectionString("Postgres") 
                                     ?? throw new InvalidOperationException("Connection string 'Postgres' is missing.");

            options.UseNpgsql(postgresConnection);
            
            if (isProduction)
            {
                options.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddFilter("", LogLevel.None)));
            }
        });
    }

    private static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IDestinationRepository, DestinationRepository>();
        services.AddScoped<IGroupInvitationRepository, GroupInvitationRepository>();
        services.AddScoped<IGroupMatchRepository, GroupMatchRepository>();
        services.AddScoped<IGroupMemberDestinationVoteRepository, GroupMemberDestinationVoteRepository>();
        services.AddScoped<IGroupMemberRepository, GroupMemberRepository>();
        services.AddScoped<IGroupPreferenceRepository, GroupPreferenceRepository>();
        services.AddScoped<IGroupRepository, GroupRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IUserGroupInvitationRepository, UserGroupInvitationRepository>();
        services.AddScoped<IUserPreferenceRepository, UserPreferenceRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserRoleRepository, UserRoleRepository>();
    }

    private static void AddClients(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IRedisClient>(sp =>
        {
            var redisConnection = configuration.GetConnectionString("Redis") 
                                  ?? throw new InvalidOperationException("Connection string 'Redis' is missing.");

            var logger = sp.GetRequiredService<ILogger<RedisClient>>();
            return new RedisClient(redisConnection, logger);
        });
        
        services.AddTransient<ISmtpClient>(sp =>
        {
            var emailSettings = configuration
                .GetRequiredSection(nameof(EmailSettings))
                .Get<EmailSettings>()!;

            var logger = sp.GetRequiredService<ILogger<SmtpClientWrapper>>();
            
            var smtpClient = new SmtpClient
            {
                Host = emailSettings.SmtpServer,
                Port = emailSettings.Port,
                Credentials = new NetworkCredential(emailSettings.Username, emailSettings.Password),
                EnableSsl = emailSettings.EnableSsl
            };

            return new SmtpClientWrapper(smtpClient, logger);
        });
    }

    private static void AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IPasswordHashService, PasswordHashService>();
        services.AddSingleton<IRandomCodeGeneratorService, RandomCodeGeneratorService>();
        services.AddSingleton<ITokenService, TokenService>();
        services.AddSingleton<IEmailTemplateService, EmailTemplateService>();
        
        services.AddScoped<IRedisService, RedisService>();
        services.AddScoped<IEmailSenderService>(sp =>
        {
            var emailSettings = configuration
                .GetRequiredSection(nameof(EmailSettings))
                .Get<EmailSettings>()!;

            var smtpClient = sp.GetRequiredService<ISmtpClient>();
            var templateService = sp.GetRequiredService<IEmailTemplateService>();
            var logger = sp.GetRequiredService<ILogger<EmailSenderService>>();
            
            return new EmailSenderService(emailSettings.From, smtpClient, templateService, logger);
        });
    }
}