using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Polly;
using Polly.Retry;
using Serilog;
using Serilog.Core;
using WeatherApp.Application.Interface;
using WeatherApp.Application.Service;
using WeatherApp.Infrastructure.Security;
using WeatherApp.Persistence.Data;

var builder = WebApplication.CreateBuilder(args);

// builder.Services.AddDbContext<AppDbContext>(opt => 
//     opt.UseInMemoryDatabase("InMemoryDatabase"));

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 43)) // Replace with your MySQL version
    )
);


builder.Services.AddScoped<IWeatherService, WeatherService>();
builder.Services.AddScoped<ITokenService, TokenService>();


builder.Services.AddResiliencePipeline("default", x =>
{
    x.AddRetry(new RetryStrategyOptions
        {
            ShouldHandle = new PredicateBuilder().Handle<Exception>(),
            Delay = TimeSpan.FromSeconds(2),
            MaxRetryAttempts = 2,
            BackoffType = DelayBackoffType.Exponential,
            UseJitter = true
        })
        .AddTimeout(TimeSpan.FromSeconds(30));
});
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpClient();

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = Encoding.UTF8.GetBytes(jwtSettings["Secret"]);

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options => {
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(secretKey)
        };
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = c =>
            {
                c.NoResult();

                Console.WriteLine(c.Exception);

                c.Response.StatusCode = 500;
                c.Response.ContentType = "text/plain";
                return c.Response.WriteAsync("An error occured processing your authentication.");
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine("JWT Authentication succeeded for " + context.Principal.Identity.Name);
                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddSecurityInfrastructure();

var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();
builder.Logging.AddSerilog(logger);
var app = builder.Build();
// app.UseHttpsRedirection();

IdentityModelEventSource.ShowPII = true;
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();


app.Run();
