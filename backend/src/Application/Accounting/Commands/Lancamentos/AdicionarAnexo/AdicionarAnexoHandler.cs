namespace BudgetCouple.Application.Accounting.Commands.Lancamentos.AdicionarAnexo;

using BudgetCouple.Application.Common.Interfaces;
using BudgetCouple.Application.Common.Interfaces.Accounting;
using BudgetCouple.Domain.Accounting.Lancamentos;
using MediatR;
using System.Threading.Tasks;

public class AdicionarAnexoHandler : IRequestHandler<AdicionarAnexoCommand, AdicionarAnexoResponse>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ILancamentoRepository _lancamentoRepository;
    private readonly ILancamentoAnexoRepository _anexoRepository;
    private readonly IAttachmentStorage _storage;

    public AdicionarAnexoHandler(
        IApplicationDbContext dbContext,
        ILancamentoRepository lancamentoRepository,
        ILancamentoAnexoRepository anexoRepository,
        IAttachmentStorage storage)
    {
        _dbContext = dbContext;
        _lancamentoRepository = lancamentoRepository;
        _anexoRepository = anexoRepository;
        _storage = storage;
    }

    public async Task<AdicionarAnexoResponse> Handle(AdicionarAnexoCommand request, CancellationToken cancellationToken)
    {
        // Verify lancamento exists
        var lancamento = await _lancamentoRepository.GetByIdAsync(request.LancamentoId, cancellationToken);
        if (lancamento == null)
            throw new InvalidOperationException($"Lançamento {request.LancamentoId} não encontrado.");

        // Save file to storage
        var caminhoStorage = await _storage.SaveAsync(
            request.LancamentoId,
            request.NomeArquivo,
            request.ArquivoStream,
            cancellationToken);

        // Create anexo entity
        var result = LancamentoAnexo.Criar(
            request.LancamentoId,
            request.NomeArquivo,
            request.ContentType,
            request.TamanhoBytes,
            caminhoStorage);

        if (result.IsFailure)
            throw new InvalidOperationException(result.Error.Message);

        var anexo = result.Value;

        // Add to repository
        await _anexoRepository.AddAsync(anexo, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new AdicionarAnexoResponse(
            anexo.Id,
            anexo.NomeArquivo,
            anexo.TamanhoBytes,
            anexo.EnviadoEm);
    }
}
