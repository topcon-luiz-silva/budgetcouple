using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

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
