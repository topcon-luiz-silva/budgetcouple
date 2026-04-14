namespace BudgetCouple.Application.Import.Commands;

using BudgetCouple.Application.Common.Interfaces;
using BudgetCouple.Application.Common.Interfaces.Accounting;
using BudgetCouple.Application.Import.DTOs;
using BudgetCouple.Domain.Accounting;
using BudgetCouple.Domain.Accounting.Lancamentos;
using BudgetCouple.Domain.Common;
using MediatR;

public class ConfirmImportHandler : IRequestHandler<ConfirmImportCommand, Result<ConfirmImportResultDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILancamentoRepository _lancamentoRepository;

    public ConfirmImportHandler(IApplicationDbContext context, ILancamentoRepository lancamentoRepository)
    {
        _context = context;
        _lancamentoRepository = lancamentoRepository;
    }

    public async Task<Result<ConfirmImportResultDto>> Handle(ConfirmImportCommand request, CancellationToken cancellationToken)
    {
        var result = new ConfirmImportResultDto();
        var lancamentosParaSalvar = new List<Lancamento>();

        // Validate account/card
        if (!request.ContaId.HasValue && !request.CartaoId.HasValue)
        {
            return Result.Failure<ConfirmImportResultDto>(
                Error.Validation("ContaId ou CartaoId deve ser informado"));
        }

        // Process each transaction
        foreach (var item in request.Lancamentos)
        {
            try
            {
                // Create lancamento (REALIZADA nature)
                var lancamentoResult = Lancamento.CriarSimples(
                    tipo: TipoLancamento.DESPESA, // Default to expense, could be enhanced
                    natureza: NaturezaLancamento.REALIZADA,
                    valor: item.Valor,
                    data: item.DataCompetencia,
                    categoriaId: item.CategoriaId,
                    subcategoriaId: item.SubcategoriaId,
                    contaId: request.ContaId,
                    cartaoId: request.CartaoId,
                    descricao: item.Descricao,
                    classificacao: ClassificacaoRecorrencia.VARIAVEL);

                if (lancamentoResult.IsFailure)
                {
                    result.Falhas++;
                    result.Mensagens.Add($"Erro ao criar lançamento '{item.Descricao}': {lancamentoResult.Error.Message}");
                    continue;
                }

                var lancamento = lancamentoResult.Value;
                // Set hash for deduplication detection
                lancamento.GetType().GetProperty("HashImportacao")?.SetValue(lancamento, item.HashImportacao);

                lancamentosParaSalvar.Add(lancamento);
                result.Sucesso++;
            }
            catch (Exception ex)
            {
                result.Falhas++;
                result.Mensagens.Add($"Erro inesperado ao processar '{item.Descricao}': {ex.Message}");
            }
        }

        // Save all transactions
        if (lancamentosParaSalvar.Any())
        {
            try
            {
                foreach (var lancamento in lancamentosParaSalvar)
                {
                    _lancamentoRepository.Add(lancamento);
                }
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                return Result.Failure<ConfirmImportResultDto>(
                    Error.Validation($"Erro ao salvar lançamentos: {ex.Message}"));
            }
        }

        return Result.Success(result);
    }
}
