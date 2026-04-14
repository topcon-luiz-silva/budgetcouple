namespace BudgetCouple.Application.Import.Commands;

using BudgetCouple.Application.Common.Interfaces;
using BudgetCouple.Application.Import.DTOs;
using BudgetCouple.Application.Import.Interfaces;
using BudgetCouple.Domain.Common;
using MediatR;
using System.Security.Cryptography;
using System.Text;

public class PreviewImportHandler : IRequestHandler<PreviewImportCommand, Result<ImportPreviewDto>>
{
    private readonly IOfxParser _ofxParser;
    private readonly ICsvParser _csvParser;
    private readonly IClassificationEngine _classificationEngine;
    private readonly IApplicationDbContext _context;

    public PreviewImportHandler(
        IOfxParser ofxParser,
        ICsvParser csvParser,
        IClassificationEngine classificationEngine,
        IApplicationDbContext context)
    {
        _ofxParser = ofxParser;
        _csvParser = csvParser;
        _classificationEngine = classificationEngine;
        _context = context;
    }

    public async Task<Result<ImportPreviewDto>> Handle(PreviewImportCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var fileName = request.FileName.ToLowerInvariant();
            var transacoes = new List<(string descricao, decimal valor, DateOnly data)>();

            // Parse file based on extension
            if (fileName.EndsWith(".ofx"))
            {
                var parsed = await _ofxParser.ParseAsync(request.FileStream);
                transacoes = parsed.Select(t => (t.Descricao, t.Valor, t.Data)).ToList();
            }
            else if (fileName.EndsWith(".csv"))
            {
                var parsed = await _csvParser.ParseAsync(request.FileStream);
                transacoes = parsed.Select(t => (t.Descricao, t.Valor, t.Data)).ToList();
            }
            else
            {
                return Result.Failure<ImportPreviewDto>(Error.Validation("Formato de arquivo não suportado. Use .ofx ou .csv"));
            }

            if (!transacoes.Any())
            {
                return Result.Failure<ImportPreviewDto>(Error.Validation("Nenhum lançamento encontrado no arquivo"));
            }

            // Get existing hashes to detect duplicates
            var dbContext = _context as Microsoft.EntityFrameworkCore.DbContext;
            var existentesHashes = dbContext.Set<BudgetCouple.Domain.Accounting.Lancamentos.Lancamento>()
                .Where(l => l.HashImportacao != null)
                .Select(l => l.HashImportacao)
                .ToHashSet();

            // Get all categories
            var categorias = dbContext.Set<BudgetCouple.Domain.Accounting.Categorias.Categoria>().ToList();

            // Build preview items
            var items = new List<ImportItemDto>();

            foreach (var (descricao, valor, data) in transacoes)
            {
                // Create hash for deduplication
                var hash = ComputeHash(request.ContaId, request.CartaoId, data, valor, descricao);
                var isDuplicado = existentesHashes.Contains(hash);

                // Classify transaction
                var classificacao = await _classificationEngine.ClassifyAsync(descricao);

                items.Add(new ImportItemDto
                {
                    Descricao = descricao,
                    Valor = valor,
                    DataCompetencia = data,
                    CategoriaSugeridaId = classificacao?.categoriaId,
                    CategoriaSugeridaNome = classificacao.HasValue
                        ? categorias.FirstOrDefault(c => c.Id == classificacao.Value.categoriaId)?.Nome
                        : null,
                    Duplicado = isDuplicado,
                    HashImportacao = hash
                });
            }

            var result = new ImportPreviewDto
            {
                Lancamentos = items
            };

            return Result.Success(result);
        }
        catch (Exception ex)
        {
            return Result.Failure<ImportPreviewDto>(Error.Validation(ex.Message));
        }
    }

    private string ComputeHash(Guid? contaId, Guid? cartaoId, DateOnly data, decimal valor, string descricao)
    {
        var origem = (contaId ?? cartaoId).ToString() ?? "";
        var sanitizada = descricao.Trim().ToLowerInvariant();
        var input = $"{origem}{data:yyyyMMdd}{valor:F2}{sanitizada}";

        using (var sha256 = SHA256.Create())
        {
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToBase64String(hashBytes);
        }
    }
}
