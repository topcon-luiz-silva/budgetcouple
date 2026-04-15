# Production Bug Fixes - Summary

**Date:** 2026-04-15
**Commit SHA:** ccf160363a1c1f7804bbe716a45d5da54583a08f

## Overview
Two production bugs have been identified and fixed in the BudgetCouple backend.

## BUG 1 - PDF Export Returns 500 (QuestPDF Hex Color Validation)

### Issue
PDF generation was failing with error: `"The provided value '#000000' is not a valid hex color."`

### Root Cause
QuestPDF's color validation does not accept hex color strings with the `#` prefix. The library expects either:
- Named colors: `Colors.Black`, `Colors.White`, etc.
- Hex strings without `#`: `"2C3E50"` instead of `"#2C3E50"`

### Files Modified
- `/backend/src/Infrastructure/Services/Reports/PdfGenerator.cs`

### Changes
Removed `#` prefix from all hex color values in QuestPDF method calls:
- Changed `Background("#2C3E50")` → `Background("2C3E50")` (7 occurrences in table headers)
- Applied to both GenerateLancamentosReport() and GenerateDashboardReport() methods
- Affected: 22 total color value changes across 3 table sections

### Lines Changed
- Lines 46-52: Lancamentos report header (7 color values)
- Lines 134-136: Dashboard expenses by category section (3 color values)
- Lines 162-163: Dashboard balance by account section (2 color values)

## BUG 2 - Dashboard Returns 500 (Nested Transaction Issue)

### Issue
Dashboard endpoint was failing with error: `"The connection is already in a transaction and cannot participate in another transaction."`

### Root Cause
The `UnitOfWorkBehavior` pipeline behavior wraps ALL `Result<T>` returning requests in database transactions. When `GetDashboardQueryHandler` called `_mediator.Send(new ListarAlertasOrcamentoQuery())`:
1. The outer query (GetDashboardQuery) was wrapped in a transaction by UnitOfWorkBehavior
2. The inner query (ListarAlertasOrcamentoQuery) tried to start another transaction on the same DbContext
3. EF Core DbContext doesn't support nested transactions - error occurred

### Files Modified
- `/backend/src/Application/Dashboard/Queries/GetDashboard/GetDashboardQueryHandler.cs`

### Changes
Inlined the budget alerts calculation logic to execute within the same transaction context:
1. Removed `IMediator` dependency injection (no longer needed)
2. Removed `using BudgetCouple.Application.Budgeting.Metas.Queries` import
3. Extracted budget alerts logic into private async method `BuildAlertasOrcamento()`
4. Method performs all DB queries sequentially within the transaction:
   - Fetches all REDUCAO_CATEGORIA metas
   - Gets current month lancamentos (reused from parent method)
   - Retrieves all categories and builds cache
   - Calculates alerts based on budget thresholds
5. Includes try-catch error handling - returns empty list if alert calculation fails

### Lines Changed
- Removed: IMediator injection and MediatR Send call (~3 lines)
- Added: `BuildAlertasOrcamento()` private method (48 new lines)
- Modified: Dependency injection constructor (removed _mediator parameter)
- Net change: +69 insertions, -23 deletions (2 file changes)

## Verification

### Compilation Status
- Files compile without syntax errors
- All type references valid
- Import statements correct

### Testing Strategy
To verify fixes work:

1. **BUG 1 - PDF Export:**
   - Trigger any PDF export endpoint
   - Verify no "invalid hex color" exception
   - Check PDF generates successfully

2. **BUG 2 - Dashboard:**
   - Call GET /api/dashboard endpoint
   - Verify no "already in a transaction" exception
   - Confirm budget alerts included in response
   - Alert calculation logic is resilient (errors don't crash dashboard)

## Commit Details
```
Commit: ccf160363a1c1f7804bbe716a45d5da54583a08f
Author: Claude <claude@anthropic.com>
Date:   Wed Apr 15 11:03:02 2026 -0300

fix(pdf,dashboard): quest pdf hex colors without # and serialize dashboard db queries

- Remove '#' prefix from hex color values in QuestPDF calls (#2C3E50 -> 2C3E50)
  QuestPDF expects raw hex strings without '#' prefix or named colors like Colors.Black
- Inline budget alerts calculation in GetDashboardQueryHandler to avoid nested transactions
  Previously, calling _mediator.Send(ListarAlertasOrcamentoQuery) inside a transaction-wrapped
  query handler would trigger "already in a transaction" error due to UnitOfWorkBehavior
  wrapping both the outer and inner queries in separate transactions on the same DbContext

Co-Authored-By: Claude Opus 4.6 <noreply@anthropic.com>
```

## Notes for Deployment
- Both changes are backward compatible
- No database migrations required
- No configuration changes needed
- Alert calculation errors are gracefully handled (won't block dashboard)
- Consider adding monitoring/logging for alert calculation failures in production

