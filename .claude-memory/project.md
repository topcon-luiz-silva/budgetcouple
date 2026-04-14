---
name: BudgetCouple Project
description: App de gestão de despesas pessoais compartilhadas — contexto técnico e referência ao PRD
type: project
---

# BudgetCouple

**Localização:** `/home/luiz-felipe/Documentos/Claude/Projects/Aplicativo de Finanças/budgetcouple/`
**PRD:** `docs/PRD-App-Despesas-Pessoais.md` (1267 linhas, 20 seções)

## Stack
- **Backend:** .NET 8 (DDD + Clean Architecture + CQRS/MediatR + Result Pattern) + EF Core 8 + Npgsql + JWT + Serilog + FluentValidation + BCrypt + xUnit
- **Frontend:** React 18 + TypeScript + Vite + TailwindCSS 3 + shadcn/ui + React Router 6 + TanStack Query v5 + Zustand (MVVM) + RHF + Zod + Recharts + date-fns + i18next + vite-plugin-pwa
- **Banco:** PostgreSQL via Supabase (free tier). RLS desabilitada; acesso controlado pela API.
- **Deploy:** Render.com (backend) + Vercel (frontend) + UptimeRobot (anti cold-start).

## Convenções
- Código em **inglês**; mensagens ao usuário e nomes de domínio em **pt-BR**.
- Conventional Commits (`feat:`, `fix:`, `chore:`, `test:`, `docs:`).
- Sem exceções para regras de negócio — usar `Result<T>`.
- Cobertura mínima 70% no Domain (xUnit + FluentAssertions + NSubstitute).
- Dashboard em **uma única chamada agregada** (RF-13).
- Cartão de crédito: regime de **caixa** — despesa no cartão só afeta conta quando fatura é paga (RF-11).
- PIN único compartilhado; casal vê exatamente os mesmos dados.

## Bounded Contexts (DDD)
1. **Identity** — PIN compartilhado (AppConfig).
2. **Accounting** (núcleo) — Contas, Cartoes, Categorias, Lancamentos, Recorrencias, Faturas.
3. **Budgeting** — Metas, regras de orçamento, alertas.
4. **ImportAndReconciliation** — Importação OFX/CSV, regras.
5. **Notifications** — Push/Email/Telegram.
6. **Reporting** — Excel/PDF/Dashboard.

## Plano de Fases (Seção 16 do PRD)
- **Fase 0** — Setup (1-2h) ✅
- **Fase 1** — Fundação Backend DDD (4-6h)
- **Fase 2** — Autenticação PIN/JWT (2-3h)
- **Fase 3** — Contas, Cartões, Categorias (3-4h)
- **Fase 4** — Lançamentos (6-8h, núcleo)
- **Fase 5** — Fatura de Cartão (3-4h)
- **Fase 6** — Dashboard e Relatórios (4-5h)
- **Fase 7** — Importação OFX/CSV (4-5h)
- **Fase 8** — Metas e Alertas (2-3h)
- **Fase 9** — Notificações (3-4h)
- **Fase 10** — Anexos, Backup, PWA, A11y (3-4h)
- **Fase 11** — Testes e Hardening (3-4h)
- **Fase 12** — Deploy (2-3h)

## Infra local
- .NET 8 SDK instalado em `~/.dotnet` (PATH via `.bashrc`)
- Node 20.20.2 + npm 10.8.2
- Git repo inicializado com branch `main`
