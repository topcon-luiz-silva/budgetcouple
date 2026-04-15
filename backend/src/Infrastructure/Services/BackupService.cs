namespace BudgetCouple.Infrastructure.Services;

using BudgetCouple.Application.Common.Interfaces;
using BudgetCouple.Domain.Accounting.Cartoes;
using BudgetCouple.Domain.Accounting.Categorias;
using BudgetCouple.Domain.Accounting.Contas;
using BudgetCouple.Domain.Accounting.Lancamentos;
using BudgetCouple.Domain.Accounting.Recorrencias;
using BudgetCouple.Domain.Budgeting.Metas;
using BudgetCouple.Domain.Identity;
using BudgetCouple.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;
using BudgetCouple.Domain.Imports;

public interface IBackupService
{
    Task<string> ExportAsync(CancellationToken cancellationToken = default);
    Task ImportAsync(string jsonData, string mode = "merge", CancellationToken cancellationToken = default);
}

public class BackupService : IBackupService
{
    private readonly AppDbContext _dbContext;

    public BackupService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<string> ExportAsync(CancellationToken cancellationToken = default)
    {
        var backup = new BackupData
        {
            Versao = "1.0",
            GeradoEm = DateTime.UtcNow,
            Contas = await _dbContext.Contas.ToListAsync(cancellationToken),
            Cartoes = await _dbContext.Cartoes.ToListAsync(cancellationToken),
            Categorias = await _dbContext.Categorias.ToListAsync(cancellationToken),
            Lancamentos = await _dbContext.Lancamentos.ToListAsync(cancellationToken),
            Recorrencias = await _dbContext.Recorrencias.ToListAsync(cancellationToken),
            Metas = await _dbContext.Metas.ToListAsync(cancellationToken),
            RegrasClassificacao = await _dbContext.RegrasClassificacao.ToListAsync(cancellationToken),
        };

        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters = { new JsonStringEnumConverter() }
        };

        return JsonSerializer.Serialize(backup, options);
    }

    public async Task ImportAsync(string jsonData, string mode = "merge", CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(jsonData))
            throw new ArgumentException("JSON data cannot be empty.");

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter() }
        };

        BackupData? backup;
        try
        {
            backup = JsonSerializer.Deserialize<BackupData>(jsonData, options);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException($"JSON inválido: {ex.Message}");
        }

        if (backup == null)
            throw new InvalidOperationException("Backup data não pôde ser desserializado.");

        await using var transaction = await _dbContext.BeginTransactionAsync(cancellationToken);
        try
        {
            if (mode == "replace")
            {
                // Truncate tables
                await _dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE lancamento_anexos CASCADE", cancellationToken);
                await _dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE lancamentos CASCADE", cancellationToken);
                await _dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE recorrencias CASCADE", cancellationToken);
                await _dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE metas CASCADE", cancellationToken);
                await _dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE regras_classificacao CASCADE", cancellationToken);
                await _dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE categorias CASCADE", cancellationToken);
                await _dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE cartoes CASCADE", cancellationToken);
                await _dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE contas CASCADE", cancellationToken);
            }

            // Import data
            if (backup.Contas?.Any() == true)
            {
                foreach (var conta in backup.Contas)
                {
                    if (mode == "merge" && await _dbContext.Contas.AnyAsync(c => c.Id == conta.Id, cancellationToken))
                        continue;
                    _dbContext.Contas.Add(conta);
                }
            }

            if (backup.Cartoes?.Any() == true)
            {
                foreach (var cartao in backup.Cartoes)
                {
                    if (mode == "merge" && await _dbContext.Cartoes.AnyAsync(c => c.Id == cartao.Id, cancellationToken))
                        continue;
                    _dbContext.Cartoes.Add(cartao);
                }
            }

            if (backup.Categorias?.Any() == true)
            {
                foreach (var categoria in backup.Categorias)
                {
                    if (mode == "merge" && await _dbContext.Categorias.AnyAsync(c => c.Id == categoria.Id, cancellationToken))
                        continue;
                    _dbContext.Categorias.Add(categoria);
                }
            }

            if (backup.Lancamentos?.Any() == true)
            {
                foreach (var lancamento in backup.Lancamentos)
                {
                    if (mode == "merge" && await _dbContext.Lancamentos.AnyAsync(l => l.Id == lancamento.Id, cancellationToken))
                        continue;
                    _dbContext.Lancamentos.Add(lancamento);
                }
            }

            if (backup.Recorrencias?.Any() == true)
            {
                foreach (var recorrencia in backup.Recorrencias)
                {
                    if (mode == "merge" && await _dbContext.Recorrencias.AnyAsync(r => r.Id == recorrencia.Id, cancellationToken))
                        continue;
                    _dbContext.Recorrencias.Add(recorrencia);
                }
            }

            if (backup.Metas?.Any() == true)
            {
                foreach (var meta in backup.Metas)
                {
                    if (mode == "merge" && await _dbContext.Metas.AnyAsync(m => m.Id == meta.Id, cancellationToken))
                        continue;
                    _dbContext.Metas.Add(meta);
                }
            }

            if (backup.RegrasClassificacao?.Any() == true)
            {
                foreach (var regra in backup.RegrasClassificacao)
                {
                    if (mode == "merge" && await _dbContext.RegrasClassificacao.AnyAsync(r => r.Id == regra.Id, cancellationToken))
                        continue;
                    _dbContext.RegrasClassificacao.Add(regra);
                }
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}

public class BackupData
{
    [JsonPropertyName("versao")]
    public string Versao { get; set; } = "1.0";

    [JsonPropertyName("geradoEm")]
    public DateTime GeradoEm { get; set; }

    [JsonPropertyName("contas")]
    public List<Conta>? Contas { get; set; }

    [JsonPropertyName("cartoes")]
    public List<Cartao>? Cartoes { get; set; }

    [JsonPropertyName("categorias")]
    public List<Categoria>? Categorias { get; set; }

    [JsonPropertyName("lancamentos")]
    public List<Lancamento>? Lancamentos { get; set; }

    [JsonPropertyName("recorrencias")]
    public List<Recorrencia>? Recorrencias { get; set; }

    [JsonPropertyName("metas")]
    public List<Meta>? Metas { get; set; }

    [JsonPropertyName("regrasClassificacao")]
    public List<RegraClassificacao>? RegrasClassificacao { get; set; }
}
