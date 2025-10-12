using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using WebApi.Clients.Implementations;
using WebApi.Clients.Interfaces;
using WebApi.Configurations;
using WebApi.Context.Implementations;
using WebApi.Context.Interfaces;
using WebApi.Persistence.Implementations;
using WebApi.Persistence.Interfaces;
using WebApi.Repositories.Implementations;
using WebApi.Repositories.Interfaces;
using WebApi.Security;
using WebApi.Services.Implementations;
using WebApi.Services.Interfaces;
using WebApi.Models;

// TODO: quebrar esse arquivo em classes menores dependencyInjection por camada
// e metodos por separacao: registerRepositories, registerServices...
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var jwtSection = builder.Configuration.GetRequiredSection(nameof(JsonWebTokenSettings));
builder.Services.Configure<JsonWebTokenSettings>(jwtSection);
var jwtSettings = jwtSection.Get<JsonWebTokenSettings>()!;

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters()
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

builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .RequireClaim(Claims.TokenType, TokenTypes.Access)
        .Build();
    
    options.AddPolicy(Policies.RegisterValidateEmail, policy =>
        policy.RequireClaim(Claims.TokenType, TokenTypes.Step)
            .RequireClaim(Claims.Step, Steps.ValidateEmail));

    options.AddPolicy(Policies.RegisterSetPassword, policy => 
        policy.RequireClaim(Claims.TokenType, TokenTypes.Step)
            .RequireClaim(Claims.Step, Steps.SetPassword));

    options.AddPolicy(Policies.ResetPassword, policy =>
        policy.RequireClaim(Claims.TokenType, TokenTypes.ResetPassword));

    options.AddPolicy(Policies.Admin, policy => 
        policy.RequireRole(Roles.Admin).RequireClaim(Claims.TokenType, TokenTypes.Access));
});

builder.Services.AddDbContext<AppDbContext>(options =>
{
    string postgresConnection = builder.Configuration.GetConnectionString("Postgres")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is missing.");

    options.UseNpgsql(postgresConnection);
});

builder.Services.AddSingleton<IRedisClient>(_ => 
{
    string redisConnection = builder.Configuration.GetConnectionString("Redis") 
        ?? throw new InvalidOperationException("Connection string 'Redis' is missing.");

    return new RedisClient(redisConnection);
});

builder.Services.AddSingleton<IPasswordHashService, PasswordHashService>();
builder.Services.AddSingleton<IRandomCodeGeneratorService, RandomCodeGeneratorService>();
builder.Services.AddSingleton<ITokenService, TokenService>();

builder.Services.AddTransient(_ =>
{
    var emailSettings = builder.Configuration
        .GetRequiredSection(nameof(EmailSettings))
        .Get<EmailSettings>()!;

    return new SmtpClient
    {
        Host = emailSettings.SmtpServer,
        Port = emailSettings.Port,
        Credentials = new NetworkCredential(emailSettings.Username, emailSettings.Password),
        EnableSsl = emailSettings.EnableSsl
    };
});

builder.Services.AddScoped<IEmailSenderService, EmailSenderService>();
builder.Services.AddScoped<IRedisService, RedisService>();

builder.Services.AddScoped<IDestinationRepository, DestinationRepository>();
builder.Services.AddScoped<IGroupInvitationRepository, GroupInvitationRepository>();
builder.Services.AddScoped<IGroupMatchRepository, GroupMatchRepository>();
builder.Services.AddScoped<IGroupMemberDestinationVoteRepository, GroupMemberDestinationVoteRepository>();
builder.Services.AddScoped<IGroupMemberRepository, GroupMemberRepository>();
builder.Services.AddScoped<IGroupRepository, GroupRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IUserGroupInvitationRepository, UserGroupInvitationRepository>();
builder.Services.AddScoped<IUserPreferenceRepository, UserPreferenceRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IDestinationRepository, DestinationRepository>();
builder.Services.AddScoped<IGroupInvitationRepository, GroupInvitationRepository>();
builder.Services.AddScoped<IGroupRepository, GroupRepository>();
builder.Services.AddScoped<IGroupMemberRepository, GroupMemberRepository>();
builder.Services.AddScoped<IGroupMemberDestinationVoteRepository, GroupMemberDestinationVoteRepository>();
builder.Services.AddScoped<IUserGroupInvitationRepository, UserGroupInvitationRepository>();
builder.Services.AddScoped<IUserPreferenceRepository, UserPreferenceRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IApplicationUserContext>(sp =>
{
    var httpContext = sp.GetRequiredService<IHttpContextAccessor>().HttpContext?.User;
    return new ApplicationUserContext(httpContext ?? new ClaimsPrincipal());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run(); 