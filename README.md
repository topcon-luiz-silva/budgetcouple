# BudgetCouple

App de Gestão de Despesas Pessoais Compartilhadas — monorepo com backend .NET 8 (DDD + Clean Architecture + CQRS) e frontend React 18 + Vite + TypeScript.

## Estrutura

```
budgetcouple/
├── backend/                         # Solution .NET 8
│   ├── BudgetCouple.sln
│   ├── src/
│   │   ├── Domain/                  # Aggregates, Value Objects, Domain Events, Result<T>
│   │   ├── Application/             # CQRS (MediatR), Validators (FluentValidation), Interfaces
│   │   ├── Infrastructure/          # EF Core (Npgsql), repositórios, serviços externos
│   │   └── Api/                     # ASP.NET Core Web API, Swagger, JWT, Serilog
│   ├── tests/
│   │   ├── Domain.UnitTests/        # xUnit
│   │   ├── Application.UnitTests/
│   │   └── Integration.Tests/
│   └── .env.example
├── frontend/
│   └── budgetcouple-web/            # Vite + React + TS + Tailwind + shadcn/ui
│       └── .env.example
├── docs/
│   └── PRD-App-Despesas-Pessoais.md
└── README.md
```

## Stack

**Backend:** .NET 8, EF Core 8 + Npgsql, MediatR, FluentValidation, Serilog, JWT Bearer, Swagger, BCrypt, xUnit.

**Frontend:** React 18, TypeScript, Vite, TailwindCSS 3, shadcn/ui, React Router, React Query (TanStack), Zustand, React Hook Form + Zod, Recharts, lucide-react, date-fns, i18next, vite-plugin-pwa, Axios.

**Banco:** PostgreSQL via Supabase.

## Pré-requisitos

- .NET 8 SDK (`dotnet --version` → 8.x)
- Node 20+ e npm 10+
- Conta Supabase (free tier)
- Git

## Setup Rápido

### 1. Clone e configure envs

```bash
git clone <repo> budgetcouple
cd budgetcouple

cp backend/.env.example backend/.env
cp frontend/budgetcouple-web/.env.example frontend/budgetcouple-web/.env
```

Edite os arquivos `.env` com sua connection string Supabase, URL/anon key, e um `JWT_SECRET` forte (≥64 chars).

### 2. Supabase

1. Crie um projeto em https://supabase.com/dashboard
2. Em **Project Settings → Database**, copie a connection string (use a pooler `aws-0-…:5432`).
3. Em **Project Settings → API**, copie `Project URL` e `anon key`.
4. Em **Storage**, crie um bucket privado chamado `budgetcouple`.
5. Cole os valores nos dois `.env`.

### 3. Backend

```bash
cd backend
dotnet restore
dotnet build
dotnet run --project src/Api/BudgetCouple.Api.csproj
# → http://localhost:5000/swagger
```

### 4. Frontend

```bash
cd frontend/budgetcouple-web
npm install
npm run dev
# → http://localhost:5173
```

## Convenções

- Código em **inglês**; mensagens ao usuário e nomes de domínio em **pt-BR**.
- Commits: Conventional Commits (`feat:`, `fix:`, `chore:`, `test:`, `docs:`).
- Sem exceções para regras de negócio — usar `Result<T>`.
- Cobertura mínima 70% no projeto `Domain` (xUnit + FluentAssertions + NSubstitute).

## Roadmap

O plano de execução por fases está em [`docs/PRD-App-Despesas-Pessoais.md`](docs/PRD-App-Despesas-Pessoais.md) (Seção 16).

Status: **Projeto 100% Completo** ✅

- [x] Fase 0 — Setup
- [x] Fase 1 — Fundação Backend DDD
- [x] Fase 2 — Autenticação
- [x] Fase 3 — Contas, Cartões, Categorias
- [x] Fase 4 — Lançamentos
- [x] Fase 5 — Fatura de Cartão
- [x] Fase 6 — Dashboard e Relatórios
- [x] Fase 7 — Importação OFX/CSV
- [x] Fase 8 — Metas e Alertas
- [x] Fase 9 — Notificações
- [x] Fase 10 — Anexos, Backup, PWA, A11y
- [x] Fase 11 — Testes e Hardening
- [x] Fase 12 — Deploy

## Deploy

Para fazer deploy em produção, consulte o guia completo em [`docs/DEPLOY.md`](docs/DEPLOY.md).

Stack de produção:
- **Backend:** Render.com (Docker)
- **Frontend:** Vercel
- **Banco:** Supabase (PostgreSQL)
- **Health Check:** UptimeRobot (anti cold-start)

## CI/CD

O repositório possui workflows GitHub Actions automatizados:

- `.github/workflows/backend-ci.yml` — Testa build e testes do backend .NET
- `.github/workflows/frontend-ci.yml` — Testa build, lint e testes do frontend React

Ambos executam automaticamente em PRs e pushes para `main`/`develop`.
