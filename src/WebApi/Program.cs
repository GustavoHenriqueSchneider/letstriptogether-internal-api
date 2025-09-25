using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using WebApi.Context;
using WebApi.Security;
using WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// TODO: JsonWebTokenSettings pode ser uma classe para facilitar import o appsettings
var secretKey = builder.Configuration["JsonWebTokenSettings:SecretKey"] ?? 
    throw new InvalidOperationException("Invalid secret key");

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
            ValidIssuer = builder.Configuration["JsonWebTokenSettings:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(secretKey))
        };
    }); 

builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .RequireClaim(Claims.TokenType, TokenTypes.Access)
        .Build();
    
    options.AddPolicy(Policies.RegisterValidateEmail, policy =>
        policy.RequireClaim(Claims.TokenType, TokenTypes.Step).RequireClaim(Claims.Step, Steps.ValidateEmail));

    options.AddPolicy(Policies.RegisterSetPassword, policy => 
        policy.RequireClaim(Claims.TokenType, TokenTypes.Step).RequireClaim(Claims.Step, Steps.SetPassword));

    options.AddPolicy(Policies.ResetPassword, policy =>
        policy.RequireClaim(Claims.TokenType, TokenTypes.ResetPassword));

    options.AddPolicy(Policies.Admin, policy => 
        policy.RequireRole(Roles.Admin).RequireClaim(Claims.TokenType, TokenTypes.Access));
});

string postgresConnection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(postgresConnection));

builder.Services.AddSingleton<ITokenService, TokenService>();
builder.Services.AddSingleton<IPasswordHashService, PasswordHashService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IApplicationUserContext>(sp =>
{
    var httpContext = sp.GetRequiredService<IHttpContextAccessor>().HttpContext;
    return new ApplicationUserContext(httpContext?.User ?? new ClaimsPrincipal());
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