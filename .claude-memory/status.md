---
name: BudgetCouple Status
description: Status atual das fases do projeto e próximos passos
type: project
---

# Status de Execução

**Última atualização:** 2026-04-14

## Fases concluídas
- ✅ **Fase 0 — Setup** (2026-04-14): monorepo `budgetcouple/`, .NET 8 SDK instalado, solution com 7 projetos (Domain/Application/Infrastructure/Api + 3 testes) com referências entre camadas, frontend Vite + TS com todas as libs (Tailwind, shadcn/ui base, React Router, React Query, Zustand, RHF+Zod, Recharts, lucide, date-fns, i18next, vite-plugin-pwa, axios), pacotes backend (MediatR, FluentValidation, EF Core + Npgsql, BCrypt, Serilog, JwtBearer, Swashbuckle), `.env.example` backend/frontend, README.md, .gitignore, commit inicial. Ambos builds verificados (0 erros).

## Próximas fases
- ⏳ **Fase 1** — Fundação Backend DDD (em execução)
- ⏳ Fases 2-12 pendentes

## Como retomar em sessão futura
1. `cd "/home/luiz-felipe/Documentos/Claude/Projects/Aplicativo de Finanças/budgetcouple/"`
2. Ler `.claude-memory/project.md` + este arquivo + `docs/PRD-App-Despesas-Pessoais.md`
3. Rodar `git log --oneline` para ver último commit feito
4. `export PATH=$HOME/.dotnet:$PATH` (se abrir novo shell)
