using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BudgetCouple.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "app_config",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    pin_hash = table.Column<string>(type: "varchar(255)", nullable: false),
                    pin_set_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    failed_attempts = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    locked_until = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    tema = table.Column<string>(type: "varchar(20)", nullable: false, defaultValue: "light"),
                    email_notificacao = table.Column<string>(type: "varchar(255)", nullable: true),
                    telegram_chat_ids = table.Column<List<string>>(type: "text[]", nullable: false, defaultValue: new List<string>())
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_app_config", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "cartoes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "varchar(100)", nullable: false),
                    bandeira = table.Column<string>(type: "varchar(20)", nullable: false),
                    dia_fechamento = table.Column<int>(type: "integer", nullable: false),
                    dia_vencimento = table.Column<int>(type: "integer", nullable: false),
                    limite = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    icone = table.Column<string>(type: "varchar(50)", nullable: true),
                    cor = table.Column<string>(type: "varchar(7)", nullable: false),
                    ativa = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cartoes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "categorias",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "varchar(100)", nullable: false),
                    tipo = table.Column<string>(type: "varchar(20)", nullable: false),
                    icone = table.Column<string>(type: "varchar(50)", nullable: true),
                    cor = table.Column<string>(type: "varchar(7)", nullable: false),
                    sistema = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    ativa = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categorias", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "contas",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "varchar(100)", nullable: false),
                    tipo = table.Column<string>(type: "varchar(20)", nullable: false),
                    saldo_inicial = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    descricao = table.Column<string>(type: "varchar(255)", nullable: true),
                    icone = table.Column<string>(type: "varchar(50)", nullable: true),
                    cor = table.Column<string>(type: "varchar(7)", nullable: false),
                    ativa = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contas", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "lancamentos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tipo = table.Column<string>(type: "varchar(20)", nullable: false),
                    natureza = table.Column<string>(type: "varchar(20)", nullable: false),
                    valor = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    data = table.Column<DateOnly>(type: "date", nullable: false),
                    conta_id = table.Column<Guid>(type: "uuid", nullable: true),
                    cartao_id = table.Column<Guid>(type: "uuid", nullable: true),
                    categoria_id = table.Column<Guid>(type: "uuid", nullable: false),
                    subcategoria_id = table.Column<Guid>(type: "uuid", nullable: true),
                    descricao = table.Column<string>(type: "varchar(255)", nullable: true),
                    tags = table.Column<List<string>>(type: "text[]", nullable: false, defaultValue: new List<string>()),
                    status_pagamento = table.Column<string>(type: "varchar(12)", nullable: false),
                    data_pagamento = table.Column<DateOnly>(type: "date", nullable: true),
                    is_parcelada = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    parcela_atual = table.Column<int>(type: "integer", nullable: true),
                    total_parcelas = table.Column<int>(type: "integer", nullable: true),
                    lancamento_pai_id = table.Column<Guid>(type: "uuid", nullable: true),
                    is_recorrente = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    recorrencia_id = table.Column<Guid>(type: "uuid", nullable: true),
                    anexos = table.Column<List<string>>(type: "text[]", nullable: false, defaultValue: new List<string>()),
                    classificacao = table.Column<string>(type: "varchar(20)", nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lancamentos", x => x.id);
                    table.CheckConstraint("chk_origem", "(conta_id IS NOT NULL AND cartao_id IS NULL) OR (conta_id IS NULL AND cartao_id IS NOT NULL) OR (conta_id IS NULL AND cartao_id IS NULL AND natureza = 'PREVISTA')");
                });

            migrationBuilder.CreateTable(
                name: "metas",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tipo = table.Column<string>(type: "varchar(20)", maxLength: 21, nullable: false),
                    nome = table.Column<string>(type: "varchar(100)", nullable: false),
                    valor_alvo = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    valor_atual = table.Column<decimal>(type: "numeric(18,2)", nullable: false, defaultValue: 0m),
                    data_limite = table.Column<DateOnly>(type: "date", nullable: true),
                    categoria_id = table.Column<Guid>(type: "uuid", nullable: true),
                    periodo = table.Column<string>(type: "varchar(10)", nullable: true),
                    percentual_alerta = table.Column<int>(type: "integer", nullable: false, defaultValue: 80),
                    ativa = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_metas", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "recorrencias",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    frequencia = table.Column<string>(type: "varchar(20)", nullable: false),
                    data_inicio = table.Column<DateOnly>(type: "date", nullable: false),
                    data_fim = table.Column<DateOnly>(type: "date", nullable: true),
                    template_json = table.Column<string>(type: "text", nullable: false),
                    ativa = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recorrencias", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "regras_classificacao",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    padrao = table.Column<string>(type: "varchar(200)", nullable: false),
                    tipo_padrao = table.Column<string>(type: "varchar(10)", nullable: false),
                    categoria_id = table.Column<Guid>(type: "uuid", nullable: false),
                    subcategoria_id = table.Column<Guid>(type: "uuid", nullable: true),
                    prioridade = table.Column<int>(type: "integer", nullable: false, defaultValue: 100),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_regras_classificacao", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "subcategorias",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "varchar(100)", nullable: false),
                    ativa = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CategoriaId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subcategorias", x => x.id);
                    table.ForeignKey(
                        name: "FK_subcategorias_categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "categorias",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "categorias",
                columns: new[] { "id", "ativa", "atualizado_em", "cor", "criado_em", "icone", "nome", "tipo" },
                values: new object[,]
                {
                    { new Guid("550e8400-e29b-41d4-a716-446655440001"), true, new DateTime(2026, 4, 14, 20, 22, 32, 223, DateTimeKind.Utc).AddTicks(5792), "#DC2626", new DateTime(2026, 4, 14, 20, 22, 32, 223, DateTimeKind.Utc).AddTicks(5792), "home", "Moradia", "DESPESA" },
                    { new Guid("550e8400-e29b-41d4-a716-446655440002"), true, new DateTime(2026, 4, 14, 20, 22, 32, 223, DateTimeKind.Utc).AddTicks(5820), "#F97316", new DateTime(2026, 4, 14, 20, 22, 32, 223, DateTimeKind.Utc).AddTicks(5820), "utensils", "Alimentação", "DESPESA" },
                    { new Guid("550e8400-e29b-41d4-a716-446655440003"), true, new DateTime(2026, 4, 14, 20, 22, 32, 223, DateTimeKind.Utc).AddTicks(5840), "#EAB308", new DateTime(2026, 4, 14, 20, 22, 32, 223, DateTimeKind.Utc).AddTicks(5840), "car", "Transporte", "DESPESA" },
                    { new Guid("550e8400-e29b-41d4-a716-446655440004"), true, new DateTime(2026, 4, 14, 20, 22, 32, 223, DateTimeKind.Utc).AddTicks(5858), "#EC4899", new DateTime(2026, 4, 14, 20, 22, 32, 223, DateTimeKind.Utc).AddTicks(5858), "heart-pulse", "Saúde", "DESPESA" },
                    { new Guid("550e8400-e29b-41d4-a716-446655440005"), true, new DateTime(2026, 4, 14, 20, 22, 32, 223, DateTimeKind.Utc).AddTicks(5875), "#3B82F6", new DateTime(2026, 4, 14, 20, 22, 32, 223, DateTimeKind.Utc).AddTicks(5875), "graduation-cap", "Educação", "DESPESA" },
                    { new Guid("550e8400-e29b-41d4-a716-446655440006"), true, new DateTime(2026, 4, 14, 20, 22, 32, 223, DateTimeKind.Utc).AddTicks(5893), "#8B5CF6", new DateTime(2026, 4, 14, 20, 22, 32, 223, DateTimeKind.Utc).AddTicks(5893), "film", "Lazer", "DESPESA" },
                    { new Guid("550e8400-e29b-41d4-a716-446655440007"), true, new DateTime(2026, 4, 14, 20, 22, 32, 223, DateTimeKind.Utc).AddTicks(5910), "#06B6D4", new DateTime(2026, 4, 14, 20, 22, 32, 223, DateTimeKind.Utc).AddTicks(5910), "repeat", "Assinaturas", "DESPESA" },
                    { new Guid("550e8400-e29b-41d4-a716-446655440008"), true, new DateTime(2026, 4, 14, 20, 22, 32, 223, DateTimeKind.Utc).AddTicks(5927), "#D946EF", new DateTime(2026, 4, 14, 20, 22, 32, 223, DateTimeKind.Utc).AddTicks(5927), "shirt", "Vestuário", "DESPESA" },
                    { new Guid("550e8400-e29b-41d4-a716-446655440009"), true, new DateTime(2026, 4, 14, 20, 22, 32, 223, DateTimeKind.Utc).AddTicks(5944), "#14B8A6", new DateTime(2026, 4, 14, 20, 22, 32, 223, DateTimeKind.Utc).AddTicks(5944), "paw-print", "Pets", "DESPESA" },
                    { new Guid("550e8400-e29b-41d4-a716-44665544000a"), true, new DateTime(2026, 4, 14, 20, 22, 32, 223, DateTimeKind.Utc).AddTicks(5961), "#F43F5E", new DateTime(2026, 4, 14, 20, 22, 32, 223, DateTimeKind.Utc).AddTicks(5961), "gift", "Presentes", "DESPESA" },
                    { new Guid("550e8400-e29b-41d4-a716-44665544000b"), true, new DateTime(2026, 4, 14, 20, 22, 32, 223, DateTimeKind.Utc).AddTicks(5978), "#64748B", new DateTime(2026, 4, 14, 20, 22, 32, 223, DateTimeKind.Utc).AddTicks(5978), "receipt", "Impostos e Taxas", "DESPESA" },
                    { new Guid("550e8400-e29b-41d4-a716-44665544000c"), true, new DateTime(2026, 4, 14, 20, 22, 32, 223, DateTimeKind.Utc).AddTicks(6001), "#6366F1", new DateTime(2026, 4, 14, 20, 22, 32, 223, DateTimeKind.Utc).AddTicks(6001), "landmark", "Serviços Financeiros", "DESPESA" },
                    { new Guid("550e8400-e29b-41d4-a716-44665544000d"), true, new DateTime(2026, 4, 14, 20, 22, 32, 223, DateTimeKind.Utc).AddTicks(6017), "#78716C", new DateTime(2026, 4, 14, 20, 22, 32, 223, DateTimeKind.Utc).AddTicks(6017), "more-horizontal", "Outros", "DESPESA" },
                    { new Guid("550e8400-e29b-41d4-a716-44665544000e"), true, new DateTime(2026, 4, 14, 20, 22, 32, 223, DateTimeKind.Utc).AddTicks(6035), "#10B981", new DateTime(2026, 4, 14, 20, 22, 32, 223, DateTimeKind.Utc).AddTicks(6035), "wallet", "Salário", "RECEITA" },
                    { new Guid("550e8400-e29b-41d4-a716-44665544000f"), true, new DateTime(2026, 4, 14, 20, 22, 32, 223, DateTimeKind.Utc).AddTicks(6052), "#84CC16", new DateTime(2026, 4, 14, 20, 22, 32, 223, DateTimeKind.Utc).AddTicks(6052), "award", "Bonificação", "RECEITA" },
                    { new Guid("550e8400-e29b-41d4-a716-446655440010"), true, new DateTime(2026, 4, 14, 20, 22, 32, 223, DateTimeKind.Utc).AddTicks(6069), "#06B6D4", new DateTime(2026, 4, 14, 20, 22, 32, 223, DateTimeKind.Utc).AddTicks(6069), "trending-up", "Rendimentos", "RECEITA" },
                    { new Guid("550e8400-e29b-41d4-a716-446655440011"), true, new DateTime(2026, 4, 14, 20, 22, 32, 223, DateTimeKind.Utc).AddTicks(6085), "#8B5CF6", new DateTime(2026, 4, 14, 20, 22, 32, 223, DateTimeKind.Utc).AddTicks(6085), "briefcase", "Freelance", "RECEITA" },
                    { new Guid("550e8400-e29b-41d4-a716-446655440012"), true, new DateTime(2026, 4, 14, 20, 22, 32, 223, DateTimeKind.Utc).AddTicks(6102), "#14B8A6", new DateTime(2026, 4, 14, 20, 22, 32, 223, DateTimeKind.Utc).AddTicks(6102), "undo-2", "Reembolso", "RECEITA" },
                    { new Guid("550e8400-e29b-41d4-a716-446655440013"), true, new DateTime(2026, 4, 14, 20, 22, 32, 223, DateTimeKind.Utc).AddTicks(6119), "#78716C", new DateTime(2026, 4, 14, 20, 22, 32, 223, DateTimeKind.Utc).AddTicks(6119), "more-horizontal", "Outros", "RECEITA" }
                });

            migrationBuilder.InsertData(
                table: "categorias",
                columns: new[] { "id", "ativa", "atualizado_em", "cor", "criado_em", "icone", "nome", "sistema", "tipo" },
                values: new object[] { new Guid("550e8400-e29b-41d4-a716-446655440014"), true, new DateTime(2026, 4, 14, 20, 22, 32, 223, DateTimeKind.Utc).AddTicks(6136), "#0891B2", new DateTime(2026, 4, 14, 20, 22, 32, 223, DateTimeKind.Utc).AddTicks(6136), "credit-card", "Pagamento de Fatura de Cartão", true, "DESPESA" });

            migrationBuilder.CreateIndex(
                name: "ix_lanc_cartao_data",
                table: "lancamentos",
                columns: new[] { "cartao_id", "data" });

            migrationBuilder.CreateIndex(
                name: "ix_lanc_cat_data",
                table: "lancamentos",
                columns: new[] { "categoria_id", "data" });

            migrationBuilder.CreateIndex(
                name: "ix_lanc_conta_data",
                table: "lancamentos",
                columns: new[] { "conta_id", "data" });

            migrationBuilder.CreateIndex(
                name: "ix_lanc_data",
                table: "lancamentos",
                column: "data");

            migrationBuilder.CreateIndex(
                name: "IX_subcategorias_CategoriaId",
                table: "subcategorias",
                column: "CategoriaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "app_config");

            migrationBuilder.DropTable(
                name: "cartoes");

            migrationBuilder.DropTable(
                name: "contas");

            migrationBuilder.DropTable(
                name: "lancamentos");

            migrationBuilder.DropTable(
                name: "metas");

            migrationBuilder.DropTable(
                name: "recorrencias");

            migrationBuilder.DropTable(
                name: "regras_classificacao");

            migrationBuilder.DropTable(
                name: "subcategorias");

            migrationBuilder.DropTable(
                name: "categorias");
        }
    }
}
