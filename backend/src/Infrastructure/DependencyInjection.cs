namespace BudgetCouple.Infrastructure;

using BudgetCouple.Application.Common.Interfaces;
using BudgetCouple.Application.Common.Interfaces.Accounting;
using BudgetCouple.Infrastructure.Persistence;
using BudgetCouple.Infrastructure.Persistence.Repositories;
using BudgetCouple.Infrastructure.Services;
using BudgetCouple.Infrastructure.Services.Auth;
using BudgetCouple.Infrastructure.Services.Reports;
using BudgetCouple.Infrastructure.Services.Import;
using BudgetCouple.Application.Import.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default") ??
            throw new InvalidOperationException("Connection string 'Default' not found.");

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<AppDbContext>());
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());

        // Register repositories
        services.AddScoped<IAppConfigRepository, AppConfigRepository>();
        services.AddScoped<IContaRepository, ContaRepository>();
        services.AddScoped<ICartaoRepository, CartaoRepository>();
        services.AddScoped<ICategoriaRepository, CategoriaRepository>();
        services.AddScoped<ILancamentoRepository, LancamentoRepository>();
        services.AddScoped<IRecorrenciaRepository, RecorrenciaRepository>();
        services.AddScoped<IRegraClassificacaoRepository, RegraClassificacaoRepository>();

        // Register services
        services.AddScoped<IPinHasher, PinHasher>();
        services.AddScoped<IDateTimeProvider, DateTimeProvider>();
        services.AddScoped<IExcelGenerator, ExcelGenerator>();
        services.AddScoped<IPdfGenerator, PdfGenerator>();

        // Register import services
        services.AddScoped<IOfxParser, OfxParser>();
        services.AddScoped<ICsvParser, CsvParser>();
        services.AddScoped<IClassificationEngine, ClassificationEngine>();

        // Register JWT token service
        services.Configure<JwtTokenOptions>(options =>
        {
            var jwtConfig = configuration.GetSection("Jwt");
            options.Issuer = jwtConfig["Issuer"] ?? "BudgetCouple";
            options.Audience = jwtConfig["Audience"] ?? "BudgetCouple";
            options.Secret = jwtConfig["Secret"] ?? throw new InvalidOperationException("JWT Secret not configured");
            if (int.TryParse(jwtConfig["ExpiresMinutes"], out var expiresMinutes))
            {
                options.ExpiresMinutes = expiresMinutes;
            }
        });
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        return services;
    }
}
