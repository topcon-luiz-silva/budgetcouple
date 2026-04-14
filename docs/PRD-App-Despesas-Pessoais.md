# PRD — App de Gestão de Despesas Pessoais Compartilhadas

**Versão:** 1.0
**Data:** 14/04/2026
**Autor:** Luiz Felipe (Product Owner) + Claude (Engenheiro/PO)
**Status:** Aguardando aprovação final para início do desenvolvimento
**Audiência:** Claude Code (executor) + Luiz Felipe (aprovador)
**Codinome interno:** `BudgetCouple`

---

## Sumário

1. Visão Geral e Contexto
2. Problema de Negócio e Resultado Esperado
3. Personas e Usuários
4. User Stories (Visão Funcional)
5. Requisitos Funcionais Detalhados
6. Requisitos Não Funcionais
7. Arquitetura e Stack Tecnológica
8. Modelo de Domínio (DDD)
9. Modelo de Dados (SQL)
10. Contratos de API REST
11. Lista Completa de Commands e Queries (CQRS)
12. Estrutura de Pastas — Backend (.NET)
13. Estrutura de Pastas — Frontend (React)
14. Protótipos de Tela Low-Fi
15. Cenários de Teste (Gherkin)
16. Plano de Execução em Fases
17. Prompt Master para o Claude Code
18. Checklist de Entrega e Critérios de Aceite Global
19. Estimativa de Esforço
20. Riscos e Mitigações

---

## 1. Visão Geral e Contexto

Aplicação **web PWA** (instalável no celular e acessível no desktop) para controle de finanças domésticas compartilhadas entre **dois usuários** (casal). O app permite:

- Cadastro de contas, cartões de crédito, categorias e subcategorias.
- Lançamento de receitas e despesas (previstas e realizadas).
- Controle de despesas fixas (recorrentes) e variáveis.
- Comparação **orçado vs. realizado** em dashboards e gráficos.
- Controle de faturas de cartão de crédito com parcelamento.
- Importação de extratos (OFX/CSV) com conciliação opcional.
- Metas de economia e redução de categoria.
- Notificações multi-canal (Push PWA, E-mail, Telegram).
- Tema claro/escuro, idioma pt-BR, moeda Real (BRL).

Dados persistidos em **Supabase (PostgreSQL)** — plano gratuito. Backend em **.NET 8 (C#)** com arquitetura **DDD + Clean Architecture + CQRS**. Frontend em **React 18 + TypeScript + Vite** com padrão **MVVM** (stores como ViewModel). Hospedagem: backend em **Render.com** (free tier), frontend em **Vercel** (free tier).

Acesso protegido por **PIN único compartilhado** configurável.

---

## 2. Problema de Negócio e Resultado Esperado

### 2.1 Problema de Negócio

Luiz Felipe e esposa não têm hoje uma ferramenta unificada para controle financeiro do casal. Planilhas manuais são trabalhosas, se desatualizam e não oferecem comparação automática entre o que foi orçado e o que foi realmente gasto. Apps do mercado (Mobills, Organizze, Money Lover) são pagos para recursos essenciais, ou exigem cadastro em bancos, ou não atendem o modelo de finanças conjuntas simples. Como consequência:

- Falta de visibilidade de para onde o dinheiro está indo.
- Dificuldade em identificar categorias que estouram o orçamento.
- Impossibilidade de projetar sobra mensal/anual com confiabilidade.
- Cartão de crédito com faturas parceladas gera confusão sobre o caixa real do mês.

### 2.2 Resultado Esperado

Ao final do projeto, o casal terá um app próprio, personalizado e gratuito que:

- Permite lançar despesas em poucos segundos (desktop ou celular).
- Calcula automaticamente orçado vs. realizado por categoria.
- Exibe dashboards com maiores despesas, tendências e alertas de estouro.
- Mantém fatura de cartão separada do caixa e projeta impacto futuro.
- Dá previsibilidade financeira mensal e anual (projeção de sobra/investimento).
- Funciona offline-first (PWA) e sincroniza entre celular e desktop.

---

## 3. Personas e Usuários

| Persona | Descrição | Necessidades principais |
|---------|-----------|--------------------------|
| **Luiz Felipe (Titular)** | Entusiasta de tecnologia, gerencia finanças do casal, usa desktop e mobile. | Velocidade no lançamento, dashboards ricos, importação de extrato, exportação. |
| **Esposa (Co-titular)** | Usa principalmente o mobile, quer interface simples. | Lançamento rápido, visão consolidada, notificações de vencimento. |

Ambos compartilham **o mesmo PIN** e **veem exatamente os mesmos dados** (finanças conjuntas, sem separação de autoria).

---

## 4. User Stories

| ID | Ator | Necessidade | Finalidade / Valor |
|----|------|-------------|--------------------|
| US-01 | Usuário | Fazer login com PIN único | Proteger acesso aos dados financeiros |
| US-02 | Usuário | Cadastrar contas bancárias e carteiras | Segregar saldos por origem |
| US-03 | Usuário | Cadastrar cartões de crédito com fechamento e vencimento | Controlar faturas |
| US-04 | Usuário | Gerenciar categorias (pré-cadastradas + criar novas) | Classificar receitas e despesas |
| US-05 | Usuário | Gerenciar subcategorias opcionais | Detalhar gastos quando desejado |
| US-06 | Usuário | Lançar uma despesa prevista | Planejar orçamento futuro |
| US-07 | Usuário | Lançar uma despesa realizada a partir de uma previsão | Fechar o ciclo orçado vs. realizado |
| US-08 | Usuário | Lançar despesa recorrente (fixa) com opção de editar mês a mês | Automatizar contas fixas |
| US-09 | Usuário | Lançar compra no cartão de crédito (à vista ou parcelada) | Registrar impacto na fatura futura |
| US-10 | Usuário | Pagar a fatura do cartão | Baixar a fatura contra uma conta |
| US-11 | Usuário | Lançar receitas previstas e realizadas | Controlar renda |
| US-12 | Usuário | Importar extrato OFX/CSV com conciliação opcional | Agilizar lançamentos em massa |
| US-13 | Usuário | Visualizar dashboard mensal e anual | Ter visão consolidada |
| US-14 | Usuário | Ver comparativo orçado vs. realizado por categoria | Identificar estouros |
| US-15 | Usuário | Cadastrar metas de economia e de redução de categoria | Atingir objetivos financeiros |
| US-16 | Usuário | Anexar comprovantes às transações | Guardar prova fiscal |
| US-17 | Usuário | Receber notificações de vencimento em Push/E-mail/Telegram | Não atrasar contas |
| US-18 | Usuário | Exportar relatórios em Excel e PDF | Arquivar e compartilhar |
| US-19 | Usuário | Alternar tema claro/escuro | Conforto visual |
| US-20 | Usuário | Trocar o PIN | Manter segurança |

---

## 5. Requisitos Funcionais Detalhados

### 5.1 Autenticação e Acesso

- **RF-01:** No primeiro acesso ao app, o sistema solicita a criação de um PIN numérico de 6 a 8 dígitos. O PIN é único e compartilhado por ambos os usuários.
- **RF-02:** O PIN é armazenado no banco como hash (BCrypt, custo 12). Nunca em texto claro.
- **RF-03:** Após 5 tentativas consecutivas erradas, o acesso fica bloqueado por 15 minutos.
- **RF-04:** A sessão autenticada usa **JWT** (token válido por 30 dias com refresh automático). O token é armazenado em `httpOnly cookie` quando possível; caso contrário, `localStorage` cifrado.
- **RF-05:** Deve haver tela de "Trocar PIN" dentro de Configurações, pedindo PIN atual + novo PIN + confirmação.
- **RF-06:** Ao encerrar navegador por mais de 30 dias, o sistema exige reentrada do PIN.

### 5.2 Contas e Cartões de Crédito

- **RF-07:** O usuário pode cadastrar múltiplas **contas** com: Nome, Tipo (`CONTA_CORRENTE`, `POUPANCA`, `CARTEIRA`, `INVESTIMENTO`), Instituição (texto livre), Saldo inicial, Cor e Ícone.
- **RF-08:** O usuário pode cadastrar múltiplos **cartões de crédito** com: Nome, Bandeira, Últimos 4 dígitos (opcional), Limite, Dia de fechamento (1–31), Dia de vencimento (1–31), Conta de débito padrão (FK para `Conta`), Cor.
- **RF-09:** O saldo de uma **conta** é calculado dinamicamente: `saldo_inicial + Σ receitas_realizadas − Σ despesas_realizadas (exceto cartão) − Σ pagamentos_de_fatura`.
- **RF-10:** Para cartão de crédito, o sistema calcula automaticamente a **fatura aberta** (competência atual) e a **próxima fatura** com base nos dias de fechamento.
- **RF-11:** Despesas lançadas no cartão **não afetam o saldo da conta** no momento do lançamento. O impacto ocorre apenas quando a fatura é paga (regime de caixa, conforme decisão do PO).

### 5.3 Categorias e Subcategorias

- **RF-12:** O sistema cria automaticamente no primeiro acesso as categorias padrão:
  - **Despesa:** Moradia, Alimentação, Transporte, Saúde, Educação, Lazer, Assinaturas, Vestuário, Pets, Presentes, Impostos e Taxas, Serviços Financeiros, Outros.
  - **Receita:** Salário, Bonificação, Rendimentos, Freelance, Reembolso, Outros.
- **RF-13:** O usuário pode criar, renomear e desativar categorias (não excluir se houver lançamentos; apenas desativar/arquivar).
- **RF-14:** Cada categoria tem: Nome, Tipo (`DESPESA` ou `RECEITA`), Cor (hex), Ícone (nome lucide-react), Ativa (bool).
- **RF-15:** Subcategorias são opcionais. Cada subcategoria pertence a uma categoria e herda tipo/cor (opcional sobrescrever).
- **RF-16:** O campo "descrição/tag livre" do lançamento é opcional e serve como observação do usuário.

### 5.4 Lançamentos (Receitas e Despesas)

- **RF-17:** Cada lançamento possui:
  - `Id` (GUID), `Tipo` (`RECEITA`/`DESPESA`), `Natureza` (`PREVISTA`/`REALIZADA`),
  - `ClassificacaoRecorrencia` (`FIXA`/`VARIAVEL`),
  - `Valor` (decimal ≥ 0), `Data` (data de competência/vencimento),
  - `ContaId` (nullable), `CartaoId` (nullable — se preenchido, `ContaId` fica nulo),
  - `CategoriaId` (obrigatório), `SubcategoriaId` (opcional),
  - `Descricao` (opcional, até 255 chars), `Tags` (array opcional de strings),
  - `StatusPagamento` (`PENDENTE`/`PAGO`/`ATRASADO`),
  - `DataPagamento` (nullable),
  - `IsParcelada` (bool), `ParcelaAtual` (int), `TotalParcelas` (int), `LancamentoPaiId` (GUID nullable — agrupa parcelas),
  - `IsRecorrente` (bool), `RecorrenciaId` (GUID nullable — agrupa ocorrências geradas),
  - `Anexos` (lista de URLs no Supabase Storage),
  - `CriadoEm`, `AtualizadoEm`.
- **RF-18:** Validações de regras de negócio:
  - Um lançamento **não pode** ter `ContaId` e `CartaoId` preenchidos simultaneamente.
  - Um lançamento **deve** ter `ContaId` OU `CartaoId` preenchido, exceto se for **PREVISTA** (nesse caso a origem pode ser definida depois).
  - `TotalParcelas` e `ParcelaAtual` só são válidos se `IsParcelada=true`.
- **RF-19:** Ao marcar uma despesa **PREVISTA** como **REALIZADA** (US-07), o sistema cria um **novo lançamento** do tipo `REALIZADA` vinculado à previsão via `LancamentoPaiId`, preservando o histórico da previsão. A previsão é marcada como `PAGO` e o realizado fica como registro financeiro efetivo.
- **RF-20:** Se a data de vencimento de um lançamento `PENDENTE` passar e ele não for marcado como pago, o sistema automaticamente atualiza o status para `ATRASADO` (via job diário).

### 5.5 Despesas Recorrentes (Fixas)

- **RF-21:** Ao criar um lançamento marcado como `FIXA`, o usuário escolhe a **frequência** (`MENSAL`, `BIMESTRAL`, `TRIMESTRAL`, `SEMESTRAL`, `ANUAL`) e o **horizonte** (ex: 12 ocorrências, ou "até cancelar"). O sistema cria um registro em `Recorrencias` e gera automaticamente os lançamentos futuros como `PREVISTA`.
- **RF-22:** O usuário pode **editar uma ocorrência individual** sem afetar as demais (ex: aluguel sobe em setembro).
- **RF-23:** O usuário pode **encerrar a recorrência** a partir de uma data; ocorrências futuras são excluídas.

### 5.6 Cartão de Crédito e Parcelamento

- **RF-24:** Compras parceladas geram N lançamentos filhos com `LancamentoPaiId` apontando para um lançamento pai. A descrição de cada parcela recebe o sufixo ` - Parc. X/Y` automaticamente.
- **RF-25:** A data de cada parcela é calculada pela **competência da fatura**: compra feita antes do fechamento entra na próxima fatura; após o fechamento entra na fatura seguinte.
- **RF-26:** A tela "Fatura do Cartão" lista todos os lançamentos do ciclo, soma o total, mostra a data de vencimento e permite a ação **"Pagar Fatura"**.
- **RF-27:** "Pagar Fatura" cria um lançamento de **Despesa Realizada** na conta de débito selecionada com categoria "Pagamento de Fatura de Cartão" (categoria interna do sistema) e marca todos os lançamentos da fatura com `StatusPagamento = PAGO` + `DataPagamento`.
- **RF-28:** É possível **pagamento parcial da fatura** (valor < total). Nesse caso, os lançamentos não são marcados como pagos automaticamente — fica registrado um saldo devedor no cartão.

### 5.7 Orçamento (Budget)

- **RF-29:** O orçamento é calculado automaticamente como a **soma das despesas PREVISTAS** de cada categoria em um mês. Não há cadastro separado de "meta orçamentária por categoria".
- **RF-30:** A dashboard apresenta, por categoria: **Orçado** (soma de previstas) vs. **Realizado** (soma de realizadas), com barra de progresso colorida (verde ≤ 80%, amarelo 80–100%, vermelho > 100%).

### 5.8 Importação OFX/CSV

- **RF-31:** O sistema aceita upload de arquivos **OFX** (Open Financial Exchange) e **CSV** (colunas mapeáveis).
- **RF-32:** Após upload, o sistema exibe tela com pré-visualização de todas as transações detectadas e sugestão de categoria baseada em **regras de classificação**.
- **RF-33:** O usuário pode escolher entre **"Importar tudo"** (sem revisar) ou **"Revisar e conciliar"** (tela de conciliação linha a linha).
- **RF-34:** Na conciliação, o usuário pode: alterar categoria/subcategoria, alterar descrição, marcar para ignorar, vincular a uma despesa PREVISTA existente (batimento).
- **RF-35:** O usuário pode marcar **"salvar como regra"** (ex: descrição contendo "IFOOD" → Alimentação) — regras aprendidas são aplicadas em importações futuras.

### 5.9 Metas

- **RF-36:** **Meta de Economia**: Nome, Valor alvo, Data limite, Valor acumulado atual (editável manualmente), Barra de progresso.
- **RF-37:** **Meta de Redução de Categoria**: Categoria, Valor teto, Período (mensal). O sistema alerta quando o realizado do mês passa de X% (configurável, padrão 80%) do teto.

### 5.10 Anexos

- **RF-38:** Cada lançamento aceita até **5 anexos** (imagens JPG/PNG ou PDF, máximo 5 MB cada).
- **RF-39:** Anexos ficam no **Supabase Storage**, bucket privado, com URL assinada de 1h para visualização.

### 5.11 Notificações

- **RF-40:** Três canais configuráveis independentemente por usuário: **Push PWA**, **E-mail** (via Resend free tier), **Telegram Bot** (usuário cadastra `chat_id` nas configurações).
- **RF-41:** Eventos notificáveis:
  - Lançamento `PENDENTE` com vencimento em **3 dias**, **1 dia** e **no dia**.
  - Categoria atingindo **80%** ou **100%** do orçado no mês.
  - Fatura de cartão fechada (notifica valor total).
  - Receita prevista não confirmada 3 dias após a data.

### 5.12 Relatórios e Exportação

- **RF-42:** Exportação **Excel (.xlsx)** com abas: Resumo, Lançamentos, Orçado vs. Realizado por Categoria, Cartões.
- **RF-43:** Exportação **PDF** com dashboard mensal visual (gráficos e tabelas).
- **RF-44:** Filtros de relatório: período, contas, cartões, categorias, tipo (receita/despesa), natureza (prevista/realizada).

### 5.13 Dashboard

- **RF-45:** Dashboard principal exibe:
  - **KPIs**: Receita Total (realizada), Despesa Total (realizada), Saldo do Mês, % do Orçado Consumido, Projeção de Sobra, Saldo Anual Acumulado.
  - **Gráfico Pizza**: despesas por categoria no mês.
  - **Gráfico Barras**: Orçado vs. Realizado por categoria (top 10).
  - **Gráfico Linha**: evolução de receita × despesa nos últimos 12 meses.
  - **Top 5** maiores gastos do mês.
  - **Distribuição Fixo vs. Variável** (donut).
  - **Alertas ativos**: categorias estouradas, contas atrasadas, fatura próxima do vencimento.

### 5.14 Configurações

- **RF-46:** Tela de Configurações com abas:
  - **Geral**: tema (claro/escuro/auto), idioma (pt-BR — fixo), moeda (BRL — fixo).
  - **Segurança**: trocar PIN.
  - **Contas e Cartões**: CRUD completo.
  - **Categorias e Subcategorias**: CRUD completo.
  - **Notificações**: ativar/desativar canais, configurar `chat_id` Telegram, e-mail de notificação, dias de antecedência.
  - **Recorrências**: listar e gerenciar todas as recorrências ativas.
  - **Regras de Classificação**: listar, editar e excluir regras aprendidas.
  - **Dados**: exportar todos os dados em JSON (backup), importar backup, apagar tudo.

---

## 6. Requisitos Não Funcionais

- **RNF-01 (Performance)**: tempo de resposta de APIs ≤ 300 ms p95 (CRUD simples). Dashboard ≤ 800 ms p95.
- **RNF-02 (Segurança)**: HTTPS obrigatório; JWT com assinatura HS256; BCrypt para PIN; Supabase RLS desabilitada (acesso controlado pela API); CORS restrito ao domínio frontend; rate limiting de 100 req/min por IP.
- **RNF-03 (Disponibilidade)**: aceitar downtime planejado; free tier pode ter cold start no backend (Render) — mitigado por ping agendado (uptime robot free).
- **RNF-04 (Acessibilidade)**: WCAG 2.1 nível AA (contraste mínimo, navegação por teclado, labels em formulários).
- **RNF-05 (Responsividade)**: mobile-first (360 px+), tablet, desktop (1280 px+).
- **RNF-06 (PWA)**: manifest.json + service worker com cache de shell + offline para visualização (lançamento offline fica em fila e sincroniza).
- **RNF-07 (LGPD)**: dados do usuário ficam apenas no banco do próprio usuário (Supabase). Política de privacidade no app. Função "apagar tudo" disponível.
- **RNF-08 (Observabilidade)**: logs estruturados (Serilog) com níveis, correlation ID por request, métricas básicas (tempo de resposta), Sentry free tier para erros frontend.
- **RNF-09 (Internacionalização)**: i18n configurado (react-i18next), mesmo que apenas pt-BR esteja ativo, facilitando adição futura.
- **RNF-10 (Testabilidade)**: cobertura mínima 70% no domínio (xUnit), testes de integração para commands críticos, Playwright para E2E das 5 jornadas principais.

---

## 7. Arquitetura e Stack Tecnológica

### 7.1 Stack Backend (.NET 8)

| Camada | Tecnologia | Finalidade |
|--------|------------|------------|
| Runtime | .NET 8 LTS | Base |
| Framework | ASP.NET Core Web API | HTTP API |
| ORM | Entity Framework Core 8 + Npgsql | PostgreSQL |
| CQRS/Mediator | **MediatR** | Commands, Queries, Notifications |
| Validação | **FluentValidation** | Validação declarativa |
| Mapeamento | **Mapster** | DTO ↔ Entity |
| Autenticação | **JWT** (`System.IdentityModel.Tokens.Jwt`) | Tokens |
| Hash | **BCrypt.Net-Next** | PIN |
| Logs | **Serilog** + sink Console/Seq | Estruturado |
| Documentação | **Swashbuckle (OpenAPI)** | Swagger UI |
| Testes | **xUnit + FluentAssertions + NSubstitute** | Unit/Integration |
| Arquivo OFX | **OfxSharp** (ou parser próprio) | Leitura OFX |
| Arquivo CSV | **CsvHelper** | Leitura CSV |
| Excel | **ClosedXML** | Exportação .xlsx |
| PDF | **QuestPDF** (licença community) | Exportação PDF |
| Background Jobs | **Hangfire** (in-memory ou PostgreSQL) | Jobs diários |
| Storage | **Supabase.Storage** (via REST) | Anexos |
| E-mail | **Resend** (API REST) | Notificações |
| Push | **Web Push Protocol** (`WebPush` lib) | Notificações PWA |
| Telegram | Bot API via HttpClient | Notificações |

### 7.2 Stack Frontend (React 18)

| Camada | Tecnologia | Finalidade |
|--------|------------|------------|
| Linguagem | **TypeScript 5** | Tipagem |
| Build | **Vite 5** | Dev server e bundler |
| Framework | **React 18** | UI |
| Roteamento | **React Router 6** | SPA routing |
| Estado servidor | **TanStack Query (React Query) v5** | Cache, refetch, invalidação |
| Estado UI/ViewModel | **Zustand** | Stores (padrão MVVM) |
| UI Kit | **TailwindCSS 3** + **shadcn/ui** | Componentes acessíveis |
| Ícones | **lucide-react** | Ícones |
| Gráficos | **Recharts** | Pizza, barras, linha, donut |
| Forms | **React Hook Form** + **Zod** | Formulários validados |
| Datas | **date-fns** (pt-BR) | Manipulação |
| Máscara | **react-number-format** | Moeda e datas |
| i18n | **react-i18next** | pt-BR |
| PWA | **vite-plugin-pwa** (Workbox) | Service Worker |
| Testes | **Vitest + React Testing Library + Playwright** | Unit e E2E |
| Observabilidade | **Sentry** (free) | Erros |

### 7.3 Arquitetura Geral

```
+--------------------------+          +-------------------------------+
|  Frontend React (Vercel) |  HTTPS   |  Backend .NET 8 (Render)      |
|  PWA instalável          | <------> |  ASP.NET Core Web API         |
|  - UI / ViewModels (Zus) |   JWT    |  - Controllers (REST)         |
|  - React Query           |          |  - MediatR (CQRS)             |
|  - shadcn/ui + Tailwind  |          |  - Application (Use Cases)    |
+--------------------------+          |  - Domain (DDD)               |
                                      |  - Infra (EF Core, Services)  |
                                      +-------------------------------+
                                                |        |        |
                                                v        v        v
                                      +-----------+ +----------+ +----------+
                                      |  Supabase | | Resend   | | Telegram |
                                      | Postgres  | | (email)  | |  Bot API |
                                      |  Storage  | +----------+ +----------+
                                      +-----------+
```

### 7.4 Padrões Arquiteturais Aplicados

- **DDD (Domain-Driven Design)**: Bounded Contexts, Aggregates, Entities, Value Objects, Domain Services, Domain Events.
- **Clean Architecture**: camadas `Domain → Application → Infrastructure → API`. Dependências apontam sempre para dentro.
- **CQRS via MediatR**: cada caso de uso é um `Command` (escrita) ou `Query` (leitura), com `Handler` isolado. Pipeline behaviors para logging, validação e transações.
- **Result Pattern** (`Result<T>`) em vez de exceções para regras de negócio.
- **Repository Pattern** para aggregates (não para todas as entities), encapsulado por `IUnitOfWork` do EF.
- **Specification Pattern** em queries complexas de domínio.
- **Frontend MVVM**: componentes React são a **View**, custom hooks + Zustand stores são o **ViewModel**, e os DTOs/Model vêm da API.

---

## 8. Modelo de Domínio (DDD)

### 8.1 Bounded Contexts

1. **Identity** — autenticação por PIN compartilhado.
2. **Accounting** (núcleo) — Contas, Cartões, Categorias, Subcategorias, Lançamentos, Recorrências, Faturas.
3. **Budgeting** — Metas, regras de orçamento, alertas.
4. **ImportAndReconciliation** — Importação OFX/CSV, regras de classificação.
5. **Notifications** — Dispatch multi-canal.
6. **Reporting** — Exportação e agregações.

### 8.2 Aggregates Principais

#### Aggregate: Conta
- Raiz: `Conta`
- Props: `Id`, `Nome`, `Tipo`, `Instituicao`, `SaldoInicial`, `Cor`, `Icone`, `Ativa`
- Invariantes: `Nome` não vazio, `SaldoInicial` ≥ 0.

#### Aggregate: Cartao
- Raiz: `Cartao`
- Props: `Id`, `Nome`, `Bandeira`, `UltimosDigitos`, `Limite`, `DiaFechamento`, `DiaVencimento`, `ContaDebitoId`, `Cor`, `Ativa`
- Invariantes: `1 ≤ DiaFechamento ≤ 31`, `1 ≤ DiaVencimento ≤ 31`, `Limite > 0`.
- Método de domínio: `CalcularCompetenciaFatura(dataCompra)` retorna `MesCompetencia`.

#### Aggregate: Categoria
- Raiz: `Categoria` com coleção de `Subcategoria`.
- Invariantes: subcategoria só pode existir dentro da categoria; tipo (RECEITA/DESPESA) imutável após criação.

#### Aggregate: Lancamento
- Raiz: `Lancamento`
- Value Objects: `Dinheiro` (valor + moeda), `PeriodoCompetencia`, `DadosParcelamento` (parcelaAtual, totalParcelas, lancamentoPaiId).
- Props completas conforme RF-17.
- Invariantes conforme RF-18.
- Métodos de domínio:
  - `Pagar(DataPagamento data, ContaId? contaDebito)` — valida e altera status.
  - `MarcarComoAtrasado()` — apenas se pendente e `Data < hoje`.
  - `GerarRealizadoAPartirDePrevisto(valorReal, dataReal, contaDebito)` — cria lançamento filho REALIZADA.

#### Aggregate: Recorrencia
- Raiz: `Recorrencia`
- Props: `Id`, `Frequencia`, `DataInicio`, `DataFim` (nullable), `LancamentoTemplate` (snapshot dos dados), `Ativa`.
- Método: `GerarProximasOcorrencias(ate)` → lista de `Lancamento`.

#### Aggregate: Fatura (entidade calculada, não persistida)
- Composto dinamicamente a partir de `Lancamento` do `Cartao` na competência.
- Método: `Pagar(valorPago, contaDebito)` → gera `Lancamento` de pagamento de fatura.

#### Aggregate: Meta
- Raiz: `Meta`
- Subtipos: `MetaEconomia`, `MetaReducaoCategoria` (polimorfismo via Table-Per-Hierarchy).

#### Aggregate: RegraClassificacao
- Props: `Id`, `Padrao` (string contains/regex), `CategoriaId`, `SubcategoriaId?`, `Prioridade`.

### 8.3 Domain Events

- `LancamentoCriado`, `LancamentoPago`, `LancamentoAtrasado`
- `FaturaFechada`, `FaturaPaga`
- `CategoriaOrcamentoAtingiu80`, `CategoriaOrcamentoEstourou`
- `MetaAtingida`
- `RecorrenciaGerouLancamentos`

Handlers de domain events disparam notificações (email/push/telegram) via `INotificationDispatcher`.

---

## 9. Modelo de Dados (SQL PostgreSQL)

Schema `public` no Supabase.

```sql
-- Identidade (PIN único compartilhado)
CREATE TABLE app_config (
    id                UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    pin_hash          TEXT NOT NULL,
    pin_set_at        TIMESTAMPTZ NOT NULL DEFAULT now(),
    failed_attempts   INT NOT NULL DEFAULT 0,
    locked_until      TIMESTAMPTZ,
    tema              TEXT NOT NULL DEFAULT 'AUTO',
    email_notificacao TEXT,
    telegram_chat_ids TEXT[] DEFAULT ARRAY[]::TEXT[]
);

-- Contas
CREATE TABLE contas (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    nome            VARCHAR(80) NOT NULL,
    tipo            VARCHAR(30) NOT NULL,
    instituicao     VARCHAR(80),
    saldo_inicial   NUMERIC(18,2) NOT NULL DEFAULT 0,
    cor             VARCHAR(7) NOT NULL DEFAULT '#2563EB',
    icone           VARCHAR(40) NOT NULL DEFAULT 'wallet',
    ativa           BOOLEAN NOT NULL DEFAULT true,
    criado_em       TIMESTAMPTZ NOT NULL DEFAULT now(),
    atualizado_em   TIMESTAMPTZ NOT NULL DEFAULT now()
);

-- Cartões de Crédito
CREATE TABLE cartoes (
    id                 UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    nome               VARCHAR(80) NOT NULL,
    bandeira           VARCHAR(30) NOT NULL,
    ultimos_digitos    VARCHAR(4),
    limite             NUMERIC(18,2) NOT NULL,
    dia_fechamento     INT NOT NULL CHECK (dia_fechamento BETWEEN 1 AND 31),
    dia_vencimento     INT NOT NULL CHECK (dia_vencimento BETWEEN 1 AND 31),
    conta_debito_id    UUID REFERENCES contas(id),
    cor                VARCHAR(7) NOT NULL DEFAULT '#DC2626',
    ativa              BOOLEAN NOT NULL DEFAULT true,
    criado_em          TIMESTAMPTZ NOT NULL DEFAULT now(),
    atualizado_em      TIMESTAMPTZ NOT NULL DEFAULT now()
);

-- Categorias
CREATE TABLE categorias (
    id        UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    nome      VARCHAR(60) NOT NULL,
    tipo      VARCHAR(10) NOT NULL,
    cor       VARCHAR(7) NOT NULL,
    icone     VARCHAR(40) NOT NULL,
    ativa     BOOLEAN NOT NULL DEFAULT true,
    sistema   BOOLEAN NOT NULL DEFAULT false
);

-- Subcategorias
CREATE TABLE subcategorias (
    id            UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    categoria_id  UUID NOT NULL REFERENCES categorias(id) ON DELETE CASCADE,
    nome          VARCHAR(60) NOT NULL,
    cor           VARCHAR(7),
    icone         VARCHAR(40),
    ativa         BOOLEAN NOT NULL DEFAULT true
);

-- Recorrências
CREATE TABLE recorrencias (
    id                  UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    frequencia          VARCHAR(12) NOT NULL,
    data_inicio         DATE NOT NULL,
    data_fim            DATE,
    template_json       JSONB NOT NULL,
    ativa               BOOLEAN NOT NULL DEFAULT true,
    criado_em           TIMESTAMPTZ NOT NULL DEFAULT now()
);

-- Lançamentos
CREATE TABLE lancamentos (
    id                          UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tipo                        VARCHAR(10) NOT NULL,
    natureza                    VARCHAR(10) NOT NULL,
    classificacao_recorrencia   VARCHAR(10) NOT NULL,
    valor                       NUMERIC(18,2) NOT NULL CHECK (valor >= 0),
    data                        DATE NOT NULL,
    conta_id                    UUID REFERENCES contas(id),
    cartao_id                   UUID REFERENCES cartoes(id),
    categoria_id                UUID NOT NULL REFERENCES categorias(id),
    subcategoria_id             UUID REFERENCES subcategorias(id),
    descricao                   VARCHAR(255),
    tags                        TEXT[] DEFAULT ARRAY[]::TEXT[],
    status_pagamento            VARCHAR(12) NOT NULL,
    data_pagamento              DATE,
    is_parcelada                BOOLEAN NOT NULL DEFAULT false,
    parcela_atual               INT,
    total_parcelas              INT,
    lancamento_pai_id           UUID REFERENCES lancamentos(id),
    is_recorrente               BOOLEAN NOT NULL DEFAULT false,
    recorrencia_id              UUID REFERENCES recorrencias(id),
    anexos                      TEXT[] DEFAULT ARRAY[]::TEXT[],
    criado_em                   TIMESTAMPTZ NOT NULL DEFAULT now(),
    atualizado_em               TIMESTAMPTZ NOT NULL DEFAULT now(),
    CONSTRAINT chk_origem CHECK (
        (conta_id IS NOT NULL AND cartao_id IS NULL) OR
        (conta_id IS NULL AND cartao_id IS NOT NULL) OR
        (conta_id IS NULL AND cartao_id IS NULL AND natureza = 'PREVISTA')
    )
);
CREATE INDEX ix_lanc_data ON lancamentos(data);
CREATE INDEX ix_lanc_cartao_data ON lancamentos(cartao_id, data);
CREATE INDEX ix_lanc_conta_data ON lancamentos(conta_id, data);
CREATE INDEX ix_lanc_cat_data ON lancamentos(categoria_id, data);

-- Pagamentos de Fatura
CREATE TABLE pagamentos_fatura (
    id                UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    cartao_id         UUID NOT NULL REFERENCES cartoes(id),
    competencia       DATE NOT NULL,
    valor_pago        NUMERIC(18,2) NOT NULL,
    data_pagamento    DATE NOT NULL,
    conta_debito_id   UUID NOT NULL REFERENCES contas(id),
    lancamento_id     UUID REFERENCES lancamentos(id)
);

-- Metas
CREATE TABLE metas (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tipo            VARCHAR(20) NOT NULL,
    nome            VARCHAR(100) NOT NULL,
    valor_alvo      NUMERIC(18,2) NOT NULL,
    valor_atual     NUMERIC(18,2) NOT NULL DEFAULT 0,
    data_limite     DATE,
    categoria_id    UUID REFERENCES categorias(id),
    periodo         VARCHAR(10),
    percentual_alerta INT DEFAULT 80,
    ativa           BOOLEAN NOT NULL DEFAULT true
);

-- Regras de Classificação
CREATE TABLE regras_classificacao (
    id                 UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    padrao             VARCHAR(200) NOT NULL,
    tipo_padrao        VARCHAR(10) NOT NULL DEFAULT 'CONTAINS',
    categoria_id       UUID NOT NULL REFERENCES categorias(id),
    subcategoria_id    UUID REFERENCES subcategorias(id),
    prioridade         INT NOT NULL DEFAULT 100,
    criado_em          TIMESTAMPTZ NOT NULL DEFAULT now()
);

-- Push subscriptions
CREATE TABLE push_subscriptions (
    id            UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    endpoint      TEXT NOT NULL,
    p256dh        TEXT NOT NULL,
    auth          TEXT NOT NULL,
    criado_em     TIMESTAMPTZ NOT NULL DEFAULT now()
);
```

---

## 10. Contratos de API REST

Prefixo: `/api/v1`

### 10.1 Autenticação

| Método | Rota | Descrição |
|--------|------|-----------|
| POST | `/auth/setup-pin` | Define o PIN inicial (se ainda não existe) |
| POST | `/auth/login` | Login com PIN → retorna JWT |
| POST | `/auth/change-pin` | Troca PIN (requer PIN atual) |
| GET  | `/auth/status` | Retorna se PIN já foi configurado |

### 10.2 Contas

| Método | Rota | Descrição |
|--------|------|-----------|
| GET    | `/contas` | Lista contas |
| GET    | `/contas/{id}` | Detalha conta (com saldo calculado) |
| POST   | `/contas` | Cria |
| PUT    | `/contas/{id}` | Atualiza |
| DELETE | `/contas/{id}` | Desativa |

### 10.3 Cartões

| Método | Rota | Descrição |
|--------|------|-----------|
| GET    | `/cartoes` | Lista |
| GET    | `/cartoes/{id}/faturas?competencia=2026-04` | Detalha fatura do mês |
| POST   | `/cartoes` | Cria |
| PUT    | `/cartoes/{id}` | Atualiza |
| POST   | `/cartoes/{id}/faturas/pagar` | Paga fatura (total ou parcial) |

### 10.4 Categorias / Subcategorias

| Método | Rota | Descrição |
|--------|------|-----------|
| GET    | `/categorias?tipo=DESPESA` | Lista |
| POST   | `/categorias` | Cria |
| PUT    | `/categorias/{id}` | Atualiza |
| DELETE | `/categorias/{id}` | Desativa |
| POST   | `/categorias/{id}/subcategorias` | Cria subcategoria |
| PUT    | `/subcategorias/{id}` | Atualiza |

### 10.5 Lançamentos

| Método | Rota | Descrição |
|--------|------|-----------|
| GET    | `/lancamentos?inicio=&fim=&tipo=&natureza=&cartaoId=&contaId=&categoriaId=` | Lista paginada |
| GET    | `/lancamentos/{id}` | Detalha |
| POST   | `/lancamentos` | Cria (com `parcelas`, `recorrencia`) |
| PUT    | `/lancamentos/{id}` | Atualiza |
| POST   | `/lancamentos/{id}/pagar` | Marca pago |
| POST   | `/lancamentos/{id}/realizar` | Cria realizado a partir de previsto |
| DELETE | `/lancamentos/{id}` | Exclui |
| POST   | `/lancamentos/{id}/anexos` | Upload de anexo |

### 10.6 Importação

| Método | Rota | Descrição |
|--------|------|-----------|
| POST | `/importacoes/upload` | Envia arquivo OFX/CSV → `importId` + preview |
| POST | `/importacoes/{id}/conciliar` | Envia correções |
| POST | `/importacoes/{id}/confirmar` | Efetiva importação |
| GET  | `/regras-classificacao` | Lista regras |
| POST | `/regras-classificacao` | Cria regra |

### 10.7 Metas, Notificações, Relatórios

| Método | Rota | Descrição |
|--------|------|-----------|
| GET/POST/PUT/DELETE | `/metas` | CRUD metas |
| POST | `/notificacoes/push/subscribe` | Registra subscription PWA |
| GET  | `/relatorios/dashboard?competencia=YYYY-MM` | Dados do dashboard |
| GET  | `/relatorios/export/excel?inicio=&fim=` | Download .xlsx |
| GET  | `/relatorios/export/pdf?inicio=&fim=` | Download .pdf |

### 10.8 Backup

| Método | Rota | Descrição |
|--------|------|-----------|
| GET  | `/dados/backup` | Exporta JSON completo |
| POST | `/dados/restore` | Importa JSON |
| DELETE | `/dados/apagar-tudo` | Apaga tudo |

---

## 11. Lista Completa de Commands e Queries (CQRS)

### 11.1 Commands (escrita)

- `SetupPinCommand`, `LoginCommand`, `ChangePinCommand`
- `CreateContaCommand`, `UpdateContaCommand`, `DeactivateContaCommand`
- `CreateCartaoCommand`, `UpdateCartaoCommand`, `PayFaturaCommand`
- `CreateCategoriaCommand`, `UpdateCategoriaCommand`, `DeactivateCategoriaCommand`
- `CreateSubcategoriaCommand`, `UpdateSubcategoriaCommand`
- `CreateLancamentoCommand` (suporta parcelas + recorrência)
- `UpdateLancamentoCommand`, `DeleteLancamentoCommand`
- `PayLancamentoCommand`, `RealizarPrevisaoCommand`
- `UploadAnexoCommand`
- `UploadImportFileCommand`, `ReconcileImportCommand`, `ConfirmImportCommand`
- `CreateRegraClassificacaoCommand`
- `CreateMetaCommand`, `UpdateMetaCommand`, `DeleteMetaCommand`
- `SubscribePushCommand`
- `ExportBackupCommand`, `RestoreBackupCommand`, `WipeAllDataCommand`

### 11.2 Queries (leitura)

- `GetAuthStatusQuery`
- `ListContasQuery`, `GetContaDetailQuery`
- `ListCartoesQuery`, `GetFaturaQuery(cartaoId, competencia)`
- `ListCategoriasQuery`, `ListSubcategoriasQuery`
- `ListLancamentosQuery`, `GetLancamentoQuery`
- `GetDashboardQuery(competencia)`
- `GetOrcadoVsRealizadoQuery(competencia)`
- `ListMetasQuery`
- `ListRegrasClassificacaoQuery`
- `GetImportPreviewQuery(importId)`
- `ExportExcelQuery(filtros)`, `ExportPdfQuery(filtros)`

### 11.3 Pipeline Behaviors

1. `LoggingBehavior`.
2. `ValidationBehavior` (FluentValidation).
3. `UnitOfWorkBehavior` (somente em Commands).
4. `AuditBehavior`.

---

## 12. Estrutura de Pastas — Backend (.NET)

```
BudgetCouple/
├── src/
│   ├── BudgetCouple.Domain/
│   │   ├── Common/               (Entity, AggregateRoot, ValueObject, DomainEvent, Result, Errors)
│   │   ├── Accounting/
│   │   │   ├── Contas/
│   │   │   ├── Cartoes/
│   │   │   ├── Categorias/
│   │   │   ├── Lancamentos/
│   │   │   ├── Recorrencias/
│   │   │   └── Faturas/
│   │   ├── Budgeting/
│   │   ├── Imports/
│   │   ├── Identity/
│   │   └── Notifications/
│   │
│   ├── BudgetCouple.Application/
│   │   ├── Common/               (PipelineBehaviors, Interfaces: IUnitOfWork, IDateTimeProvider)
│   │   ├── Auth/
│   │   ├── Contas/
│   │   ├── Cartoes/
│   │   ├── Categorias/
│   │   ├── Lancamentos/
│   │   ├── Imports/
│   │   ├── Metas/
│   │   ├── Notifications/
│   │   ├── Relatorios/
│   │   └── Backup/
│   │
│   ├── BudgetCouple.Infrastructure/
│   │   ├── Persistence/
│   │   │   ├── AppDbContext.cs
│   │   │   ├── Configurations/
│   │   │   ├── Migrations/
│   │   │   └── Repositories/
│   │   ├── Services/
│   │   │   ├── Auth/             (JwtTokenService, PinHasher)
│   │   │   ├── Storage/          (SupabaseStorageService)
│   │   │   ├── Email/            (ResendEmailService)
│   │   │   ├── Push/             (WebPushService)
│   │   │   ├── Telegram/         (TelegramBotService)
│   │   │   ├── Ofx/              (OfxParser)
│   │   │   └── Csv/              (CsvParser)
│   │   ├── BackgroundJobs/
│   │   │   ├── AtualizarStatusAtrasadosJob.cs
│   │   │   ├── GerarRecorrenciasJob.cs
│   │   │   ├── VerificarAlertasOrcamentoJob.cs
│   │   │   └── AvisoVencimentoJob.cs
│   │   └── DependencyInjection.cs
│   │
│   └── BudgetCouple.Api/
│       ├── Controllers/
│       ├── Middleware/
│       ├── Filters/
│       ├── Program.cs
│       ├── appsettings.json
│       └── Dockerfile
│
└── tests/
    ├── BudgetCouple.Domain.UnitTests/
    ├── BudgetCouple.Application.UnitTests/
    └── BudgetCouple.Integration.Tests/
```

**Princípios obrigatórios:**
- `Domain` não referencia nada além de `System.*`.
- `Application` referencia apenas `Domain`.
- `Infrastructure` referencia `Application` + `Domain`.
- `Api` referencia `Application` + `Infrastructure`.

---

## 13. Estrutura de Pastas — Frontend (React)

```
budgetcouple-web/
├── public/
│   ├── icons/ (PWA 192, 512, maskable)
│   ├── manifest.webmanifest
│   └── robots.txt
├── src/
│   ├── app/
│   │   ├── router.tsx
│   │   ├── providers.tsx
│   │   └── App.tsx
│   │
│   ├── shared/
│   │   ├── api/                   (axios instance, interceptors JWT)
│   │   ├── components/ui/         (shadcn/ui)
│   │   ├── components/layout/
│   │   ├── components/charts/     (Recharts wrappers)
│   │   ├── hooks/
│   │   ├── utils/
│   │   ├── types/
│   │   └── config/
│   │
│   ├── features/
│   │   ├── auth/
│   │   │   ├── pages/
│   │   │   ├── viewModels/        (Zustand + hooks)
│   │   │   ├── api/               (React Query hooks)
│   │   │   └── components/
│   │   ├── dashboard/
│   │   ├── contas/
│   │   ├── cartoes/
│   │   ├── categorias/
│   │   ├── lancamentos/
│   │   ├── imports/
│   │   ├── metas/
│   │   ├── relatorios/
│   │   ├── configuracoes/
│   │   └── notifications/
│   │
│   ├── i18n/pt-BR.json
│   ├── styles/index.css
│   ├── main.tsx
│   └── vite-env.d.ts
│
├── tests/
│   ├── unit/
│   └── e2e/                       (Playwright: 5 jornadas)
│
├── vite.config.ts
├── tailwind.config.ts
├── tsconfig.json
├── package.json
└── .env.example
```

### 13.1 Padrão MVVM

- **Model**: DTOs da API + respostas do React Query.
- **ViewModel**: hooks customizados + Zustand stores.
- **View**: componentes React "burros", recebem props do ViewModel.

Exemplo:
```tsx
// useDashboardViewModel.ts
export function useDashboardViewModel(competencia: string) {
  const { data, isLoading } = useDashboardQuery(competencia);
  const kpis = useMemo(() => mapKpis(data), [data]);
  const pizzaData = useMemo(() => mapPizza(data), [data]);
  return { kpis, pizzaData, isLoading };
}
```

---

## 14. Protótipos de Tela Low-Fi

### 14.1 Tela de Login (PIN)
```
+---------------------------------------------+
|               BudgetCouple                  |
|          Controle de Despesas               |
|                                             |
|            [ _ ][ _ ][ _ ][ _ ][ _ ][ _ ]   |
|                                             |
|   [ 1 ] [ 2 ] [ 3 ]                         |
|   [ 4 ] [ 5 ] [ 6 ]                         |
|   [ 7 ] [ 8 ] [ 9 ]                         |
|   [   ] [ 0 ] [ ⌫ ]                         |
|                                             |
|          [      Entrar      ]               |
+---------------------------------------------+
```

### 14.2 Dashboard
```
+------------------------------------------------------------------+
|  ☰  BudgetCouple               [< Abril / 2026 >]   🔔  ⚙        |
+------------------------------------------------------------------+
| RECEITAS   | DESPESAS   | SALDO MÊS | ORÇAMENTO  | SOBRA PROJ.  |
|  R$ 12.000 |  R$ 8.350  |  R$ 3.650 |  71% ████  |  R$ 4.100    |
+------------------------------------------------------------------+
|  DESPESAS POR CATEGORIA  [Pizza]      |  ORÇADO VS REALIZADO     |
|  Moradia    35%                       |  Moradia    █████ 100%   |
|  Alimentação 22%                      |  Alimentação ██ 45%      |
|  Transporte 12%                       |  Lazer     ████ 120% ⚠  |
+------------------------------------------------------------------+
|  EVOLUÇÃO 12 MESES [Linha Receita × Despesa]                     |
+------------------------------------------------------------------+
|  TOP 5 GASTOS       |  FIXO × VARIÁVEL   |  ALERTAS               |
|  1. Aluguel 2500    |  [Donut 60/40]     | ⚠ Lazer estourou       |
|  2. Escola  1200    |                    | ⚠ Fatura Itaú 03/05    |
+------------------------------------------------------------------+
|  [ + Novo Lançamento ]                                           |
+------------------------------------------------------------------+
```

### 14.3 Novo Lançamento
```
+--------------------------------------------------+
| Novo Lançamento                         [ × ]    |
+--------------------------------------------------+
| ( ) Receita   (•) Despesa                        |
| ( ) Prevista  (•) Realizada                      |
| Valor:      [ R$ 0,00         ]                  |
| Data:       [ 14/04/2026      ]                  |
| Categoria:  [ Alimentação   v ]                  |
| Subcateg.:  [ (opcional)    v ]                  |
| Origem:     (•) Conta  ( ) Cartão                |
|             [ Itaú CC      v ]                   |
| Descrição:  [                          ]         |
| Classif.:   (•) Variável  ( ) Fixa               |
| [ ] Compra parcelada      Parcelas: [ 6 ]        |
| [ ] Recorrente  Freq: [ Mensal v ] Horiz: [ 12 ] |
| Anexos:     [ + Adicionar ]                      |
|                                                  |
|            [ Cancelar ]  [ Salvar ]              |
+--------------------------------------------------+
```

### 14.4 Fatura de Cartão
```
+-------------------------------------------------------+
| Fatura • Itaú Black                      Maio/2026    |
| Fechamento: 28/04   Vencimento: 05/05                 |
| Total: R$ 2.340,00                                    |
+-------------------------------------------------------+
| Data     Descrição              Cat.      Valor       |
| 02/04    Supermercado Extra     Alimen.   R$ 420,00   |
| 05/04    Netflix                Assin.    R$ 55,90    |
| 08/04    Notebook (Parc. 1/6)   Eletrôn.  R$ 800,00   |
+-------------------------------------------------------+
|   [ Pagar Fatura ]   [ Pagamento Parcial ]            |
+-------------------------------------------------------+
```

### 14.5 Importação com Conciliação
```
+---------------------------------------------------------------+
| Importar Extrato                              [ × ]           |
+---------------------------------------------------------------+
| Arquivo: extrato_042026.ofx                                   |
| 47 transações detectadas                                      |
+---------------------------------------------------------------+
| [•] Revisar e conciliar       [ ] Importar tudo               |
+---------------------------------------------------------------+
| ✓ | Data  | Descrição         | Cat. sugerida  | Ação         |
| ☑ | 02/04 | IFOOD*RESTAURANTE  | Alimentação  v | [ salvar ]  |
| ☑ | 03/04 | POSTO SHELL        | Transporte   v | [ salvar ]  |
| ☐ | 04/04 | TED XYZ            | ? Selecione  v |              |
+---------------------------------------------------------------+
| [ Cancelar ]                      [ Confirmar Importação ]    |
+---------------------------------------------------------------+
```

### 14.6 Mobile — Lista de Lançamentos
```
+--------------------------+
| ← Lançamentos    🔍  ⋮   |
+--------------------------+
| [Hoje] [Mês] [Ano] [P]   |
+--------------------------+
| 14/04  Mercado           |
|        R$ 320,00  Alim.  |
|        [Cartão Itaú]     |
+--------------------------+
| 12/04  Aluguel      ✓    |
|        R$ 2.500,00 Mor.  |
+--------------------------+
| 10/04  Salário      ✓    |
|        + R$ 12.000  Sal. |
+--------------------------+
|      ( + )               |  <- FAB
+--------------------------+
| 🏠  💸  📊  ⚙            |
+--------------------------+
```

---

## 15. Cenários de Teste (Gherkin)

### 15.1 Autenticação

```
Cenário 01: Primeiro acesso define PIN
Dado que nenhum PIN foi configurado ainda
Quando o usuário informa PIN "123456" e confirma "123456"
Então o sistema salva o hash do PIN
E redireciona para o Dashboard vazio

Cenário 02: Login bem-sucedido
Dado que existe um PIN configurado "123456"
Quando o usuário informa "123456"
Então o sistema retorna JWT válido
E o usuário é redirecionado ao Dashboard

Cenário 03: Bloqueio após 5 tentativas
Dado que existe um PIN configurado "123456"
Quando o usuário erra o PIN 5 vezes consecutivas
Então o acesso é bloqueado por 15 minutos
```

### 15.2 Lançamentos

```
Cenário 04: Criar despesa no cartão parcelada
Dado que existe o cartão "Itaú Black" com fechamento dia 28
E hoje é 10/04/2026
Quando o usuário cria despesa R$ 1.200 parcelada em 6x no cartão
Então 6 lançamentos filhos são criados com R$ 200 cada
E o primeiro aparece na fatura de maio/2026
E o sexto aparece na fatura de outubro/2026
E cada descrição recebe sufixo " - Parc. X/6"

Cenário 05: Pagar fatura total
Dado que a fatura de abril/2026 soma R$ 2.340
E existe a conta "Itaú CC" com saldo R$ 5.000
Quando o usuário clica em "Pagar Fatura" debitando "Itaú CC"
Então é criado lançamento REALIZADA de R$ 2.340 na conta
E todos os lançamentos da fatura são marcados como PAGO

Cenário 06: Realizado a partir de previsão
Dado que existe previsão "Luz" R$ 180 data 10/04
Quando o usuário marca como realizado valor R$ 195 em 11/04
Então novo lançamento REALIZADA é criado com R$ 195 em 11/04
E a previsão é marcada como PAGA
```

### 15.3 Orçamento e Dashboard

```
Cenário 07: Alerta quando categoria estoura
Dado que previstas de "Lazer" somam R$ 500
E realizadas de "Lazer" somam R$ 550
Quando o usuário abre o Dashboard
Então a barra de "Lazer" aparece em vermelho (> 100%)
E alerta "Lazer estourou em R$ 50" é exibido

Cenário 08: Dashboard vazio no primeiro uso
Dado que não há lançamentos no mês
Quando o usuário abre o Dashboard
Então KPIs mostram R$ 0,00
E gráficos exibem estado vazio
```

### 15.4 Importação

```
Cenário 09: Importação OFX com conciliação
Dado que o usuário envia arquivo OFX com 10 transações
E regras casam 7 transações
Quando escolhe "Revisar e conciliar"
Então 7 transações aparecem pré-classificadas
E 3 ficam sem sugestão
Quando categoriza as 3 e confirma
Então 10 lançamentos REALIZADA são criados
```

### 15.5 Recorrência

```
Cenário 10: Criar despesa fixa mensal
Dado que hoje é 14/04/2026
Quando o usuário cria "Aluguel" R$ 2.500 FIXA MENSAL horizonte 12
Então 12 lançamentos PREVISTA são gerados (05/2026 a 04/2027)
E todos vinculados ao mesmo RecorrenciaId

Cenário 11: Editar ocorrência individual
Dado recorrência "Aluguel" com 12 ocorrências
Quando o usuário edita setembro/2026 para R$ 2.700
Então apenas setembro é alterado
E demais permanecem R$ 2.500
```

---

## 16. Plano de Execução em Fases

O Claude Code deve executar as fases **sequencialmente**, validando cada entrega antes de avançar.

### Fase 0 — Setup (1–2h)
1. Criar repositório git local `budgetcouple/` com subpastas `backend/` e `frontend/`.
2. Criar Solution .NET 8: `dotnet new sln -n BudgetCouple` + projetos `Domain`, `Application`, `Infrastructure`, `Api`, `Domain.UnitTests`, `Application.UnitTests`, `Integration.Tests`.
3. Criar projeto React: `npm create vite@latest budgetcouple-web -- --template react-ts`.
4. Configurar Tailwind, shadcn/ui, React Router, React Query, Zustand, React Hook Form, Zod, Recharts, lucide-react, date-fns, i18next, vite-plugin-pwa.
5. Criar conta Supabase, copiar connection string + URL/anon key.
6. `.env.example` em ambos os lados. README.md raiz com passo a passo.

### Fase 1 — Fundação Backend DDD (4–6h)
1. Criar `Domain/Common`: `Entity`, `AggregateRoot<TId>`, `ValueObject`, `DomainEvent`, `Result<T>`, `Error`.
2. Modelar aggregates `Conta`, `Cartao`, `Categoria`, `Lancamento`, `Recorrencia`, `Meta`, `AppConfig`.
3. Criar DbContext EF Core com `IEntityTypeConfiguration<T>` para cada entidade.
4. Migration inicial com seeds (categorias padrão + `app_config` vazio).
5. `IUnitOfWork`, repositórios por aggregate.
6. Registrar MediatR, FluentValidation, Serilog, JWT Bearer, Swagger.
7. `ExceptionMiddleware`.
8. Endpoints `GET /health` e `GET /api/v1/auth/status`.
9. **Teste**: `dotnet run` sobe, Swagger acessível, migration aplicada.

### Fase 2 — Autenticação (2–3h)
1. `SetupPinCommand`, `LoginCommand`, `ChangePinCommand` + validators.
2. `PinHasher` (BCrypt 12) e `JwtTokenService`.
3. Regras de bloqueio (5 tentativas → 15 min).
4. `AuthController`.
5. **Frontend**: páginas `PinSetupPage`, `LoginPage`; `useAuthStore`; guard de rotas; axios interceptor.
6. **Teste E2E**: setup PIN → login → troca PIN.

### Fase 3 — Contas, Cartões, Categorias (3–4h)
1. Commands/Queries + Controllers CRUD.
2. `GetContaDetailQuery` calcula saldo agregado.
3. Seed de categorias (13 despesa + 6 receita) + "Pagamento de Fatura".
4. **Frontend**: features `contas`, `cartoes`, `categorias` com tabelas shadcn `DataTable`.
5. **Teste**: CRUD via UI.

### Fase 4 — Lançamentos (6–8h) — núcleo
1. `CreateLancamentoCommand` suporta simples, parcelado, recorrente.
2. `CalcularCompetenciaFatura` no `Cartao`.
3. `PayLancamentoCommand`, `RealizarPrevisaoCommand`.
4. Job `AtualizarStatusAtrasadosJob` (Hangfire daily 00:05).
5. Job `GerarRecorrenciasJob`.
6. **Frontend**: página Lançamentos (lista paginada, filtros), modal Novo Lançamento.
7. **Teste**: cenários 04, 06, 10, 11.

### Fase 5 — Fatura de Cartão (3–4h)
1. `GetFaturaQuery(cartaoId, competencia)`.
2. `PayFaturaCommand` total/parcial.
3. **Frontend**: página Fatura.
4. **Teste**: cenário 05.

### Fase 6 — Dashboard e Relatórios (4–5h)
1. `GetDashboardQuery(competencia)` retorna tudo em uma chamada.
2. `GetOrcadoVsRealizadoQuery`.
3. `ExportExcelQuery` (ClosedXML) + `ExportPdfQuery` (QuestPDF).
4. **Frontend**: Dashboard com Recharts; Relatórios com filtros e botões export.
5. **Teste**: cenários 07 e 08.

### Fase 7 — Importação OFX/CSV (4–5h)
1. `UploadImportFileCommand` persiste no Supabase Storage.
2. Parser OFX + CSV; aplica `RegrasClassificacao`.
3. `GetImportPreviewQuery`, `ReconcileImportCommand`, `ConfirmImportCommand`.
4. **Frontend**: wizard 3 passos.
5. **Teste**: cenário 09.

### Fase 8 — Metas e Alertas (2–3h)
1. CRUD Metas (economia + redução categoria).
2. Domain events `CategoriaOrcamentoAtingiu80` e `CategoriaOrcamentoEstourou`.
3. Job `VerificarAlertasOrcamentoJob`.
4. **Frontend**: página Metas.

### Fase 9 — Notificações (3–4h)
1. Integrações Resend (e-mail), WebPush (push), Telegram Bot API.
2. `INotificationDispatcher` + handlers de domain events.
3. Job `AvisoVencimentoJob` (3/1/0 dias antes).
4. **Frontend**: Configurações → Notificações.

### Fase 10 — Anexos, Backup, PWA, A11y (3–4h)
1. Upload anexos Supabase Storage (URL assinada 1h).
2. Backup JSON completo.
3. Service Worker com cache offline do shell.
4. Revisão WCAG 2.1 AA.

### Fase 11 — Testes e Hardening (3–4h)
1. Coverage ≥ 70% no Domain.
2. Testes de integração dos 5 commands críticos.
3. Playwright E2E das 5 jornadas.
4. Rate limit, CORS, Sentry.

### Fase 12 — Deploy (2–3h)
1. **Backend**: Dockerfile + deploy no **Render.com** (free web service).
2. **Frontend**: deploy no **Vercel**.
3. UptimeRobot (free) para evitar cold start.
4. Teste E2E em produção.

---

## 17. Prompt Master para o Claude Code

Cole este prompt no Claude Code dentro da raiz do projeto:

```
Você vai construir o app BudgetCouple seguindo o PRD em /docs/PRD-App-Despesas-Pessoais.md.

REGRAS OBRIGATÓRIAS:
1. Execute as Fases 0 a 12 EM ORDEM. Ao final de cada fase, pare e peça confirmação.
2. Arquitetura: .NET 8 com DDD + Clean Architecture + CQRS (MediatR) + Result Pattern.
3. Frontend: React 18 + TypeScript + Vite + TailwindCSS + shadcn/ui + Zustand (ViewModel) + React Query.
4. Banco: PostgreSQL via Supabase (connection string em .env).
5. Código em inglês; mensagens ao usuário e nomes de domínio em português-BR.
6. Commits atômicos por tarefa, Conventional Commits (feat:, fix:, chore:, test:).
7. Cobertura mínima 70% no projeto Domain. xUnit + FluentAssertions + NSubstitute.
8. Sem exceções para regras de negócio — use Result<T>.
9. Query do dashboard deve ser uma única chamada agregada.
10. Validação via FluentValidation em todos os Commands; Zod no frontend.

ENTREGÁVEIS POR FASE:
- Código commitado.
- Testes passando.
- README atualizado.
- Changelog resumido.

Comece pela Fase 0. Antes de escrever código, liste os arquivos que vai criar/modificar.
```

---

## 18. Checklist de Entrega Global

- [ ] Usuário configura PIN e consegue logar.
- [ ] CRUD de contas, cartões, categorias e subcategorias funcional.
- [ ] Lançamento simples, parcelado e recorrente funcionando.
- [ ] Fatura de cartão com pagamento total/parcial.
- [ ] Dashboard com todos os KPIs e gráficos (RF-45).
- [ ] Importação OFX/CSV com conciliação opcional.
- [ ] Metas com alertas de 80% e 100%.
- [ ] Notificações Push, E-mail, Telegram configuráveis.
- [ ] Exportação Excel e PDF.
- [ ] Tema claro/escuro com persistência.
- [ ] PWA instalável no Android/iOS.
- [ ] Backup JSON completo.
- [ ] Deploy em produção.
- [ ] Cobertura Domain ≥ 70%.
- [ ] 5 testes E2E Playwright passando.
- [ ] README completo.

---

## 19. Estimativa de Esforço

| Fase | Descrição | Horas |
|------|-----------|------:|
| 0 | Setup | 2 |
| 1 | Fundação Backend DDD | 6 |
| 2 | Autenticação | 3 |
| 3 | Contas, Cartões, Categorias | 4 |
| 4 | Lançamentos (núcleo) | 8 |
| 5 | Fatura de Cartão | 4 |
| 6 | Dashboard e Relatórios | 5 |
| 7 | Importação OFX/CSV | 5 |
| 8 | Metas e Alertas | 3 |
| 9 | Notificações | 4 |
| 10 | Anexos, Backup, PWA, A11y | 4 |
| 11 | Testes e Hardening | 4 |
| 12 | Deploy | 3 |
| **Subtotal Dev** | | **55** |
| Code Review (10%) | | 5,5 |
| QA (20%) | | 11 |
| Buffer bugs (20%) | | 11 |
| Documentação (5%) | | 2,75 |
| Deploy & Monitoramento | | 3 |
| **TOTAL** | | **≈ 88h** |

---

## 20. Riscos e Mitigações

| Risco | Probabilidade | Impacto | Mitigação |
|-------|:-:|:-:|-----------|
| Cold start no Render free | Alta | Média | UptimeRobot ping 5 min |
| Limite Supabase (500 MB) | Baixa | Alta | Monitorar; compressão de anexos |
| Push iOS exige "Adicionar à tela" | Média | Baixa | Documentar no app |
| Parser OFX falhar com banco específico | Média | Média | Fallback CSV; logs |
| Resend free (3k/mês) | Baixa | Baixa | Priorizar push |
| Telegram — criar bot manual | Média | Baixa | Tutorial guiado |
| Segurança em URL pública | Baixa | Alta | PIN 8 dígitos, rate limit, HTTPS |

---

**FIM DO PRD v1.0**

Luiz Felipe, revise o documento e me dê a ordem para iniciar a **Fase 0** no Claude Code. Qualquer ajuste é rápido de incorporar antes do kickoff.
