using BudgetCouple.Application;
using BudgetCouple.Domain.Identity;
using BudgetCouple.Infrastructure;
using BudgetCouple.Infrastructure.Persistence;
using BudgetCouple.Infrastructure.Services.Auth;
using BudgetCouple.Api.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using QuestPDF.Infrastructure;
using Serilog;
using System.Text;

// Configure QuestPDF license
QuestPDF.Settings.License = LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables from .env file
if (File.Exists(".env"))
{
    var envVars = File.ReadAllLines(".env");
    foreach (var line in envVars)
    {
        if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
            continue;

        var parts = line.Split('=', 2);
        if (parts.Length == 2)
        {
            Environment.SetEnvironmentVariable(parts[0].Trim(), parts[1].Trim());
        }
    }
}

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

// Add services
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Configure JWT
var jwtSecret = builder.Configuration["Jwt:Secret"] ??
    throw new InvalidOperationException("JWT Secret not configured");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "BudgetCouple";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "BudgetCouple";

var key = Encoding.ASCII.GetBytes(jwtSecret);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateLifetime = true
        };
    });

builder.Services.AddAuthorization();

// Add controllers and JSON options
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

// Add CORS
var corsOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ??
    new[] { "http://localhost:5173" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("spa", builder =>
    {
        builder.WithOrigins(corsOrigins)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

var app = builder.Build();

// Apply migrations
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.Migrate();
        // Ensure initial AppConfig record exists (single-row table by design)
        if (!await dbContext.AppConfigs.AnyAsync())
        {
            dbContext.AppConfigs.Add(AppConfig.Empty());
            await dbContext.SaveChangesAsync();
        }
        logger.LogInformation("Migrations applied and AppConfig initialized");
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "Database initialization skipped (DB unavailable). /health continues to work.");
    }
}

// Middleware
app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseCors("spa");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Health endpoint
app.MapGet("/health", () => new { status = "ok", time = DateTime.UtcNow })
    .WithName("Health")
    .WithOpenApi()
    .AllowAnonymous();

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();
