namespace BudgetCouple.Application.Import.Commands;

using BudgetCouple.Application.Import.DTOs;
using BudgetCouple.Domain.Common;
using MediatR;

public class ConfirmImportCommand : IRequest<Result<ConfirmImportResultDto>>
{
    public Guid? ContaId { get; set; }
    public Guid? CartaoId { get; set; }
    public List<LancamentoImportacaoDto> Lancamentos { get; set; } = new();
}
