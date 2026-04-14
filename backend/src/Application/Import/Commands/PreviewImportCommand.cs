namespace BudgetCouple.Application.Import.Commands;

using BudgetCouple.Application.Import.DTOs;
using BudgetCouple.Domain.Common;
using MediatR;

public class PreviewImportCommand : IRequest<Result<ImportPreviewDto>>
{
    public Stream FileStream { get; set; } = null!;
    public string FileName { get; set; } = null!;
    public Guid? ContaId { get; set; }
    public Guid? CartaoId { get; set; }
}
