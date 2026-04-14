# BudgetCouple — Guia de Deploy (Fase 12)

Este documento detalha como fazer o deploy completo do BudgetCouple para produção usando:
- **Backend:** Render.com (Docker)
- **Frontend:** Vercel
- **Banco de Dados:** Supabase (PostgreSQL)
- **Health Check:** UptimeRobot (previne cold-start)

## Pré-requisitos

1. **Contas criadas:**
   - [Render.com](https://render.com) (conta gratuita)
   - [Vercel](https://vercel.com) (conta gratuita)
   - [Supabase](https://supabase.com) (conta gratuita)
   - [UptimeRobot](https://uptimerobot.com) (conta gratuita)

2. **Git:** Repositório já clonado e configurado.

3. **Domínios (opcional):** Usar subdomínios Render/Vercel ou configurar domínio customizado.

---

## 1. Banco de Dados (Supabase)

### 1.1 Criar Projeto Supabase

1. Acesse https://supabase.com/dashboard
2. Clique em **"New project"**
3. Preenchimento:
   - **Project Name:** `budgetcouple`
   - **Database Password:** Gerar senha forte (guardar com segurança!)
   - **Region:** Escolher a mais próxima geográficamente
   - **Plan:** Free tier (suficiente para MVP)
4. Aguarde deployment (~2 min)

### 1.2 Obter Connection String

1. Vá para **Project Settings → Database**
2. Em **Connection string**, copie a URL da **Connection Pooler** (importante!):
   ```
   postgresql://postgres:[PASSWORD]@[POOLER-HOST].supabase.co:6543/postgres
   ```
   - Use `[POOLER-HOST]` (não o URI padrão)
   - Substitua `[PASSWORD]` pela senha do banco

3. **Nota:** Use sempre a **pooler** para evitar límites de conexão

### 1.3 Executar Migrations

Execute as migrations no banco para criar tabelas:

```bash
cd backend

# Via Entity Framework (recomendado)
export DATABASE_URL="postgresql://postgres:[PASSWORD]@[POOLER-HOST].supabase.co:6543/postgres"
dotnet ef database update --project src/Infrastructure

# Ou via CLI (se tiver script SQL)
psql postgresql://postgres:[PASSWORD]@[POOLER-HOST].supabase.co:6543/postgres \
  -f scripts/init-database.sql
```

### 1.4 Configurar Bucket de Storage (Opcional)

Se usar upload de anexos:

1. Em **Storage → Buckets**, clique **"New bucket"**
2. Nome: `budgetcouple`
3. Privacidade: **Private**
4. Criar policy para autenticados poderem fazer upload

---

## 2. Backend — Render.com

### 2.1 Conectar Repositório

1. Acesse https://render.com
2. Clique em **"New +"** → **"Web Service"**
3. Selecione **"GitHub"** e autorize
4. Selecione repositório `budgetcouple`
5. Na aba **"Blueprint"**, selecione **Deploy from Dockerfile**

### 2.2 Configurar via render.yaml

**Alternativa (recomendada):** Use o arquivo `render.yaml` já criado:

1. No painel Render, clique **"New"** → **"Blueprint"**
2. Selecione repositório
3. Render lerá o `render.yaml` automaticamente
4. Clique **"Create New Service"**

O arquivo define:
- Dockerfile path
- Variáveis de ambiente
- Health check em `/health`
- Região: Oregon
- Plan: Free (gratuito)

### 2.3 Configurar Variáveis de Ambiente

Na página do serviço Render:

1. Vá para **"Environment"**
2. Adicione as variáveis necessárias:

| Chave | Valor |
|-------|-------|
| `DATABASE_URL` | Connection string Supabase (da seção anterior) |
| `JWT_SECRET` | Gerar com: `openssl rand -base64 64` (mínimo 64 chars) |
| `ASPNETCORE_ENVIRONMENT` | `Production` |
| `Cors__AllowedOrigins__0` | `https://budgetcouple.vercel.app` |
| `Cors__AllowedOrigins__1` | `https://localhost:3000` (dev local) |
| `SERILOG_LOGS_DIRECTORY` | `/tmp/logs` |

3. Clique **"Save"**

### 2.4 Monitorar Deploy

1. Abra a aba **"Logs"** e acompanhe:
   ```
   Installing dependencies...
   Building Docker image...
   Pushing to registry...
   Deploying...
   ```

2. Quando estiver "Live", o URL será: `https://budgetcouple-api.onrender.com`

3. Teste:
   ```bash
   curl https://budgetcouple-api.onrender.com/health
   # Resposta esperada: 200 OK
   ```

### 2.5 Troubleshooting Backend

| Problema | Solução |
|----------|---------|
| Build fails | Verificar logs; garantir que `dotnet build` funciona localmente |
| Health check fails | Confirmar que endpoint `/health` existe e não depende do DB |
| DATABASE_URL inválido | Testar locally: `psql <connection-string>` |
| Cold start longo | Configurar UptimeRobot (próxima seção) |

---

## 3. Frontend — Vercel

### 3.1 Conectar Repositório

1. Acesse https://vercel.com/dashboard
2. Clique em **"Add New..."** → **"Project"**
3. Selecione repositório `budgetcouple`
4. Configure:
   - **Framework:** Vite
   - **Root Directory:** `frontend/budgetcouple-web`
   - **Build Command:** `npm run build`
   - **Output Directory:** `dist`
   - **Install Command:** `npm install --legacy-peer-deps`

### 3.2 Configurar Variáveis de Ambiente

Na aba **"Settings"** → **"Environment Variables"**:

1. Adicione para todos os ambientes (Production, Preview, Development):

| Chave | Valor |
|-------|-------|
| `VITE_API_URL` | `https://budgetcouple-api.onrender.com/api/v1` |
| `VITE_SUPABASE_URL` | URL do seu projeto Supabase |
| `VITE_SUPABASE_ANON_KEY` | Anon key do Supabase |
| `VITE_ENABLE_WEBPUSH` | `true` (se usar notificações push) |

2. Clique **"Save"**

### 3.3 Monitorar Deploy

1. Vercel automaticamente faz deploy ao fazer push para `main`
2. Acompanhe em **"Deployments"**
3. Quando estiver "Ready", URL será: `https://budgetcouple.vercel.app`

### 3.4 Configurar Domínio Customizado (Opcional)

1. Em **"Settings"** → **"Domains"**
2. Adicione domínio próprio e configure DNS
3. Vercel fornece instruções de configuração

### 3.5 Troubleshooting Frontend

| Problema | Solução |
|----------|---------|
| Build fails | Verificar `npm run build` localmente |
| API não conecta | Confirmar `VITE_API_URL` correto em env vars |
| CORS errors | Backend deve ter CORS configurado para `budgetcouple.vercel.app` |
| Blank page | Verificar browser console (DevTools → Console) |

---

## 4. Health Check — UptimeRobot (Anti Cold-start)

### 4.1 Criar Monitor

1. Acesse https://uptimerobot.com
2. Clique em **"Add New Monitor"**
3. Configure:
   - **Monitor Type:** HTTPS
   - **Friendly Name:** BudgetCouple Health Check
   - **URL:** `https://budgetcouple-api.onrender.com/health`
   - **Monitoring Interval:** 5 minutes
   - **HTTP Method:** GET

4. Clique **"Create Monitor"**

### 4.2 Configurar Alertas (Opcional)

1. Acesse **"My Settings"** → **"Alert Contacts"**
2. Adicione email ou Slack
3. Volte ao monitor e vincule alertas

### 4.3 Por que UptimeRobot?

Render suspende serviços inativos. UptimeRobot:
- Faz ping a cada 5 min → mantém app vivo
- Alerta se houver downtime
- Evita cold-start (inicialização lenta no primeiro acesso)

---

## 5. Variáveis Sensíveis — Segurança

### 5.1 JWT Secret

Para gerar um `JWT_SECRET` robusto:

```bash
# Linux/Mac
openssl rand -base64 64

# Windows PowerShell
[Convert]::ToBase64String((1..64 | ForEach-Object { Get-Random -Maximum 256 }))
```

Copie o resultado para as variáveis de ambiente do Render.

### 5.2 Credenciais Externas (Opcional)

Se usar Telegram, Resend (email), ou WebPush:

#### Telegram Bot

1. Abra Telegram e busque por `@BotFather`
2. Envie `/newbot`
3. Siga as instruções → receba token
4. Configure em Render:
   - **Key:** `TELEGRAM_BOT_TOKEN`
   - **Value:** `<seu-token>`

#### Resend (Email)

1. Crie conta em https://resend.com
2. Gere API key em **"Settings"**
3. Configure em Render:
   - **Key:** `RESEND_API_KEY`
   - **Value:** `<sua-api-key>`

#### WebPush (Notificações)

Gere chaves VAPID localmente:

```bash
cd frontend/budgetcouple-web
npx web-push generate-vapid-keys
```

Copie as chaves para `.env` do backend:

```
VAPID_PUBLIC_KEY=<public-key>
VAPID_PRIVATE_KEY=<private-key>
VAPID_SUBJECT=mailto:seu-email@exemplo.com
```

Configure no Render.

---

## 6. Testing Pós-Deploy

### 6.1 Health Check

```bash
# Deve retornar 200 OK
curl -v https://budgetcouple-api.onrender.com/health
```

### 6.2 API Swagger

1. Acesse https://budgetcouple-api.onrender.com/swagger
2. Verifique que todos os endpoints estão documentados

### 6.3 Login

1. Abra https://budgetcouple.vercel.app
2. Registre uma conta
3. Faça login
4. Verifique que JWT token é gerado

### 6.4 CORS

1. Abra DevTools (F12 → Console)
2. Faça requisição manualmente:
   ```javascript
   fetch('https://budgetcouple-api.onrender.com/api/v1/accounts')
     .then(r => r.json())
     .then(console.log)
     .catch(console.error)
   ```
3. Não deve haver erro de CORS

### 6.5 Banco de Dados

Verifique que dados estão sendo salvos:

```bash
# Via Supabase Dashboard
# → Table Editor → Selecione uma tabela → Confirme registros
```

---

## 7. CI/CD — GitHub Actions

GitHub Actions automatiza testes a cada push:

- **.github/workflows/backend-ci.yml:** Testa backend (.NET)
- **.github/workflows/frontend-ci.yml:** Testa frontend (Node)

Ambos rodam em paralelo quando há mudanças nas respectivas pastas.

### 7.1 Verificar Status

1. Acesse GitHub → Abas **"Actions"**
2. Veja histórico de builds
3. Clique em um workflow para detalhes

### 7.2 Troubleshooting CI

Se um workflow falha:

1. Clique no workflow falho
2. Veja a aba **"Jobs"** → clique no job
3. Leia o log detalhado
4. Corrija localmente e faça push novamente

---

## 8. Rollback & Recuperação

### 8.1 Render

1. Abra o serviço → **"Logs"**
2. Em **"Deploys"**, clique no deploy anterior
3. Clique **"Redeploy"** para voltar

### 8.2 Vercel

1. Em **"Deployments"**, selecione um deployment anterior
2. Clique **"Promote to Production"**

### 8.3 Banco de Dados

Se houver erro de schema:

```bash
# Desfazer última migration
dotnet ef database update <previous-migration> \
  --project src/Infrastructure

# Ou resetar completamente (⚠️ CUIDADO — APAGA DADOS)
dotnet ef database drop
dotnet ef database update
```

---

## 9. Monitoramento em Produção

### 9.1 Logs Render

1. Dashboard Render → serviço → **"Logs"**
2. Acompanhe erros e avisos em tempo real

### 9.2 Logs Vercel

1. Dashboard Vercel → projeto → **"Logs"**
2. Veja requisições e erros de build

### 9.3 UptimeRobot Status Page

1. Abra https://uptimerobot.com → **"Status Page"**
2. Compartilhe com usuários para transparência

### 9.4 Banco Supabase

1. Dashboard Supabase → **"Logs"** e **"Replication"**
2. Monitore queries lentas

---

## 10. Atualizar Código em Produção

### 10.1 Fluxo de Deploy

1. Desenvolver localmente em branch `develop`
2. Fazer PR para `main`
3. CI/CD testa tudo (GitHub Actions)
4. Merge para `main` após aprovação
5. **Render** redeploy automaticamente (backend)
6. **Vercel** redeploy automaticamente (frontend)
7. UptimeRobot valida saúde da API

### 10.2 Deploy Parcial

Se só mudar frontend:
- Push em `frontend/` → Vercel faz redeploy
- Backend não é afetado

Se só mudar backend:
- Push em `backend/` → Render faz redeploy
- Frontend não é afetado

---

## 11. Escalabilidade Futura

### 11.1 Banco Supabase

- Free tier: ~500 MB storage, reads/writes ilimitados (rate-limited)
- Upgrade para Pro tier ($25/mês) para mais resources

### 11.2 Render

- Free tier: sleep após 15 min inatividade
- Upgrade para Starter ($7/mês) para instância dedicada sem sleep

### 11.3 Vercel

- Free tier: ilimitado (com rate limiting)
- Pro ($20/mês) para analytics avançado

---

## Checklist de Deploy

- [ ] Supabase: Banco criado e migrations rodadas
- [ ] Supabase: Connection string testada localmente
- [ ] Render: Projeto criado via Blueprint (`render.yaml`)
- [ ] Render: Variáveis de ambiente configuradas
- [ ] Render: Deploy bem-sucedido, URL acessível
- [ ] Render: Health check retorna 200 OK
- [ ] Vercel: Projeto importado, root directory correto
- [ ] Vercel: Variáveis de ambiente configuradas
- [ ] Vercel: Deploy bem-sucedido, site acessível
- [ ] Vercel: Frontend conecta corretamente ao backend
- [ ] UptimeRobot: Monitor ativo a cada 5 min
- [ ] GitHub Actions: Workflows backend e frontend verdes
- [ ] Testes: Login funciona
- [ ] Testes: CORS sem erros
- [ ] Testes: Dados salvos no banco
- [ ] Domínio customizado (opcional)

---

## Suporte

Para problemas:

1. Verificar logs (Render, Vercel, Supabase)
2. Testar endpoint localmente
3. Revisitar seção "Troubleshooting"
4. Consultar docs oficiais:
   - Render: https://render.com/docs
   - Vercel: https://vercel.com/docs
   - Supabase: https://supabase.com/docs
   - UptimeRobot: https://uptimerobot.com/help

---

**Versão:** 1.0.0
**Data:** Abril 2026
**Responsável:** BudgetCouple Team
