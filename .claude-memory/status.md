---
name: BudgetCouple Status
description: Status atual das fases do projeto e próximos passos
type: project
---

# Status de Execução

**Última atualização:** 2026-04-14

## Fases concluídas

### ✅ Fase 0 — Setup (2026-04-14)
Monorepo `budgetcouple/`, .NET 8 SDK instalado, solution com 7 projetos (Domain/Application/Infrastructure/Api + 3 testes) com referências entre camadas, frontend Vite + TS com todas as libs (Tailwind, shadcn/ui base, React Router, React Query, Zustand, RHF+Zod, Recharts, lucide, date-fns, i18next, vite-plugin-pwa, axios), pacotes backend (MediatR, FluentValidation, EF Core + Npgsql, BCrypt, Serilog, JwtBearer, Swashbuckle), `.env.example` backend/frontend, README.md, .gitignore, commit inicial.

### ✅ Fase 1 — Fundação Backend DDD (2026-04-14)
**Domain (19 files):** Common (Entity, AggregateRoot, ValueObject, DomainEvent, Result<T>, Error); Aggregates Accounting (Conta, Cartao com `CalcularCompetenciaFatura`, Categoria+Subcategoria, Lancamento com factories CriarSimples/CriarParcelado/CriarRecorrenciaOcorrencia e métodos Pagar/MarcarComoAtrasado/GerarRealizadoAPartirDePrevisto, Recorrencia com GerarProximasOcorrencias, Fatura calculada); Identity (AppConfig com Empty factory, TryLogin, RegistrarFalha, TrocarPin); Budgeting (Meta TPH com MetaEconomia e MetaReducaoCategoria); Imports (RegraClassificacao); Domain Events records.

**Application (8 files):** Interfaces (IApplicationDbContext, IUnitOfWork, ITransaction, IDateTimeProvider, ICurrentUser); Behaviors MediatR (Logging, Validation, UnitOfWork); DependencyInjection.

**Infrastructure (15 files):** AppDbContext com dispatch de domain events via IPublisher; 8 EntityTypeConfigurations (decimal(18,2), text[], enum-as-string, TPH Meta, check constraints, índices por data); Migration `InitialCreate` com schema completo + `HasData` seed de **20 categorias** (13 despesa + 6 receita + 1 sistema "Pagamento de Fatura de Cartão") usando GUIDs fixos `550e8400-e29b-41d4-a716-446655440xxx`; PinHasher (BCrypt work factor 12); JwtTokenService (HS256, 30 dias); DateTimeProvider; DependencyInjection.

**API (7 files):** Program.cs com Serilog + JWT Bearer + Swagger com JWT scheme + CORS `spa` + ExceptionMiddleware + **migrations resilientes** (try/catch, `/health` funciona sem DB) + seed AppConfig idempotente; ExceptionMiddleware (RFC 7807); AuthController com `GET /api/v1/auth/status` (stubs /setup-pin /login /change-pin para Fase 2); HealthController; appsettings.json + Development.json.

**Verificação:** `dotnet build` → 0 errors, 0 warnings. `/health` retorna 200 mesmo sem DB. Migration contém seed "Moradia" e "Pagamento de Fatura de Cartão". Commits: `feat(fase1): fundação backend DDD...` + `fix(fase1): migration resiliente + seeds...`.

**PinHash nullable:** AppConfig.pin_hash é nullable (NULL = PIN não configurado). Coluna pin_set_at também nullable. AppConfig.Empty() cria registro inicial.

### ✅ Fase 2 BACKEND — Autenticação (2026-04-14)
**Application (9 files):**
- DTOs: `AuthResult`, `AuthStatusDto`
- Commands: `SetupPinCommand` + validator + handler, `LoginCommand` + validator + handler, `ChangePinCommand` + validator + handler
- Query: `GetAuthStatusQuery` + handler
- Interfaces: `IAppConfigRepository`, `IPinHasher`, `IJwtTokenService` (abstractions)

**Infrastructure (1 file):**
- Repository: `AppConfigRepository` implementando `IAppConfigRepository`
- Updated: `PinHasher`, `JwtTokenService` para implementar interfaces do Application
- Updated: `DependencyInjection` registrando `IAppConfigRepository`

**Domain (1 file):**
- Updated: `AppConfig` com métodos `ConfigurarPin(hash, now)`, `TentativasRestantes()`, `EstaBloqueado(now)`, e `TrocarPin(hash, now)` agora recebe `DateTime`

**API (1 file):**
- Updated: `AuthController` usando MediatR + status code mapping helper
- Endpoints: `GET /status`, `POST /setup-pin`, `POST /login`, `POST /change-pin` totalmente implementados

**Tests (1 file):**
- Domain unit tests: `AppConfigTests` com 4 testes (bloqueio 5x, reset falhas, ConfigurarPin conflict, tentativas restantes)

**Verificação:** `dotnet build` → 0 errors, 0 warnings. 5 testes passando. API iniciado com sucesso (status 500 without DB, esperado).

Commit: `feat(fase2-back): autenticação PIN com JWT - SetupPin/Login/ChangePin + bloqueio 5x15min`

### ✅ Fase 2 FRONTEND — Autenticação (2026-04-14)
**shadcn/ui components (5 files):** Button (variants: default, outline, ghost, destructive; sizes: sm, default, lg), Input, Label, Card (Card + CardHeader + CardTitle + CardDescription + CardContent + CardFooter), Alert (Alert + AlertTitle + AlertDescription, variants: default/destructive).

**Stores (1 file):** `useAuthStore` (Zustand) com state { token, expiresAt }, methods { setAuth, clear, isAuthenticated() }, persistência em `localStorage.bc_token` + `bc_expires`.

**Auth API (1 file):** `authApi` typed wrappers — `getStatus()`, `setupPin(pin)`, `login(pin)`, `changePin(pinAtual, novoPin)`.

**React Query Hooks (1 file):** `useAuthStatus()` (queryKey ['auth','status'], staleTime 30s), `useSetupPin()` (mutação + setAuth + navigate /), `useLogin()` (mutação + setAuth + navigate /), `useChangePin()` (mutação + navigate /settings).

**Pages (3 files):**
- `PinSetupPage`: form RHF+Zod (6–8 digits), PIN + Confirmar PIN, error display, server errors.
- `LoginPage`: form RHF+Zod, PIN field, error display "PIN incorreto"/"Acesso bloqueado por N minutos".
- `ChangePinPage`: form RHF+Zod (PIN atual + novo PIN + confirmar), toast on success, navigate back.

**Components (2 files):**
- `RequireAuth`: route guard, redirect to /login if !isAuthenticated() (preserve from via location state).
- `AuthBootstrap`: root bootstrap, calls useAuthStatus(), handles initial routing (loading spinner, redirect /setup-pin if !pinConfigured, redirect /login if pinConfigured && !authenticated).

**App Router (updated):** `/setup-pin` → PinSetupPage, `/login` → LoginPage, `/` → RequireAuth > HomePage (welcome message "Fase 2 concluída — dashboard em breve"), `/settings/pin` → RequireAuth > ChangePinPage, `/health` → Health stub.

**API Interceptor (updated):** On 401 responses (outside /auth/* endpoints), clear localStorage + redirect to /login.

**i18n (updated):** Added keys auth.title.{setup,login,changePin}, auth.{pin,pinConfirm,pinCurrent,pinNew}, auth.submit.{setup,login,change}, auth.errors.{mismatch,invalidPin,locked}.

**Verificação:** `npm run build` → 0 errors, gzip 150.44 kB (main bundle), 9.57 kB CSS. Dev server localhost:5173 funciona. Commits: `feat(fase2-front): telas PIN setup/login/change + useAuthStore + RequireAuth + axios interceptor`.

### ✅ Fase 2 VALIDAÇÃO (2026-04-14)
Validação end-to-end via agente. Gap CRITICAL encontrado e corrigido: `Error.Forbidden` não estava mapeado no `AuthController.MapErrorToStatusCode()`, retornando 500 em vez de 403 quando conta bloqueada. Corrigido no commit `fix(fase2): mapear Error.Forbidden para HTTP 403`. Gaps menores (hardcoded i18n strings em LoginPage, falta de .max(8) em ChangePinPage.pinAtual) registrados como débito técnico a limpar na Fase 11 (hardening).

### ✅ Fase 3 BACKEND — CRUD Contas/Cartões/Categorias (2026-04-14)
**Application (48 files):** Para cada aggregate (Conta, Cartao, Categoria): Commands Create/Update/Delete + Validators + Handlers, Queries GetById/List + Handlers, DTOs, Repository interfaces.

**Infrastructure (3 files):** ContaRepository, CartaoRepository, CategoriaRepository + migration `AddContaPagamentoIdToCartao`.

**API (3 controllers):** ContasController, CartoesController, CategoriasController — 15 endpoints totais, todos com `[Authorize]` e helper `ToActionResult` com mapeamento 400/403/404/409.

**Tests:** 10 testes novos (Create handlers), todos passando. `dotnet build` 0/0. Commit `5c1315d` `feat(fase3-back): CRUD Contas/Cartões/Categorias`.

### ✅ Fase 3 FRONTEND — CRUD UI (2026-04-14)
**Components (5 novos shadcn/ui):** table, dialog, select, badge, + AppShell (sidebar com NavLinks, logout, `<Outlet/>`).

**Features (5 files × 3 módulos = 15):** features/{contas,cartoes,categorias}/{api.ts, hooks.ts, types.ts, pages/ListPage.tsx, pages/FormPage.tsx}. Todos com RHF + Zod, React Query com queryKeys, toast de erro/sucesso.

**App.tsx atualizado:** 9 novas rotas aninhadas sob AppShell. HomePage com links rápidos. i18n pt-BR ampliado (contas.*, cartoes.*, categorias.*, common.*).

**Verificação:** `npm run build` → 0 errors, 523 kB bundle (159 kB gzip). Commit `ac0e05e` `feat(fase3-front): CRUD Contas/Cartões/Categorias + AppShell + navegação`.

## Próximas fases
- ⏳ Fase 4 — Lançamentos (núcleo: simples, parcelado, recorrente, pagar, realizar)
- ⏳ Fases 5-12 pendentes

## Como retomar em sessão futura
1. `cd "/home/luiz-felipe/Documentos/Claude/Projects/Aplicativo de Finanças/budgetcouple/"`
2. Ler `.claude-memory/project.md` + este arquivo + `docs/PRD-App-Despesas-Pessoais.md`
3. `git log --oneline` para ver último commit
4. `export PATH=$HOME/.dotnet:$PATH`
