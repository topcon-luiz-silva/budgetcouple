using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetCouple.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddHashImportacaoToLancamento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Ativa",
                table: "regras_classificacao",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "AtualizadoEm",
                table: "regras_classificacao",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Nome",
                table: "regras_classificacao",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<List<string>>(
                name: "tags",
                table: "lancamentos",
                type: "text[]",
                nullable: false,
                defaultValue: new List<string>(),
                oldClrType: typeof(List<string>),
                oldType: "text[]",
                oldDefaultValue: new List<string>());

            migrationBuilder.AlterColumn<List<string>>(
                name: "anexos",
                table: "lancamentos",
                type: "text[]",
                nullable: false,
                defaultValue: new List<string>(),
                oldClrType: typeof(List<string>),
                oldType: "text[]",
                oldDefaultValue: new List<string>());

            migrationBuilder.AddColumn<string>(
                name: "HashImportacao",
                table: "lancamentos",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<List<string>>(
                name: "telegram_chat_ids",
                table: "app_config",
                type: "text[]",
                nullable: false,
                defaultValue: new List<string>(),
                oldClrType: typeof(List<string>),
                oldType: "text[]",
                oldDefaultValue: new List<string>());

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440001"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 21, 47, 51, 384, DateTimeKind.Utc).AddTicks(1406));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440002"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 21, 47, 51, 384, DateTimeKind.Utc).AddTicks(1418));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440003"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 21, 47, 51, 384, DateTimeKind.Utc).AddTicks(1428));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440004"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 21, 47, 51, 384, DateTimeKind.Utc).AddTicks(1437));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440005"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 21, 47, 51, 384, DateTimeKind.Utc).AddTicks(1446));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440006"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 21, 47, 51, 384, DateTimeKind.Utc).AddTicks(1455));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440007"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 21, 47, 51, 384, DateTimeKind.Utc).AddTicks(1464));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440008"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 21, 47, 51, 384, DateTimeKind.Utc).AddTicks(1473));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440009"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 21, 47, 51, 384, DateTimeKind.Utc).AddTicks(1482));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-44665544000a"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 21, 47, 51, 384, DateTimeKind.Utc).AddTicks(1491));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-44665544000b"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 21, 47, 51, 384, DateTimeKind.Utc).AddTicks(1500));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-44665544000c"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 21, 47, 51, 384, DateTimeKind.Utc).AddTicks(1557));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-44665544000d"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 21, 47, 51, 384, DateTimeKind.Utc).AddTicks(1567));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-44665544000e"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 21, 47, 51, 384, DateTimeKind.Utc).AddTicks(1576));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-44665544000f"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 21, 47, 51, 384, DateTimeKind.Utc).AddTicks(1585));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440010"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 21, 47, 51, 384, DateTimeKind.Utc).AddTicks(1594));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440011"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 21, 47, 51, 384, DateTimeKind.Utc).AddTicks(1603));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440012"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 21, 47, 51, 384, DateTimeKind.Utc).AddTicks(1612));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440013"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 21, 47, 51, 384, DateTimeKind.Utc).AddTicks(1621));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440014"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 21, 47, 51, 384, DateTimeKind.Utc).AddTicks(1630));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ativa",
                table: "regras_classificacao");

            migrationBuilder.DropColumn(
                name: "AtualizadoEm",
                table: "regras_classificacao");

            migrationBuilder.DropColumn(
                name: "Nome",
                table: "regras_classificacao");

            migrationBuilder.DropColumn(
                name: "HashImportacao",
                table: "lancamentos");

            migrationBuilder.AlterColumn<List<string>>(
                name: "tags",
                table: "lancamentos",
                type: "text[]",
                nullable: false,
                defaultValue: new List<string>(),
                oldClrType: typeof(List<string>),
                oldType: "text[]",
                oldDefaultValue: new List<string>());

            migrationBuilder.AlterColumn<List<string>>(
                name: "anexos",
                table: "lancamentos",
                type: "text[]",
                nullable: false,
                defaultValue: new List<string>(),
                oldClrType: typeof(List<string>),
                oldType: "text[]",
                oldDefaultValue: new List<string>());

            migrationBuilder.AlterColumn<List<string>>(
                name: "telegram_chat_ids",
                table: "app_config",
                type: "text[]",
                nullable: false,
                defaultValue: new List<string>(),
                oldClrType: typeof(List<string>),
                oldType: "text[]",
                oldDefaultValue: new List<string>());

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440001"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 21, 14, 20, 3, DateTimeKind.Utc).AddTicks(3472));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440002"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 21, 14, 20, 3, DateTimeKind.Utc).AddTicks(3483));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440003"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 21, 14, 20, 3, DateTimeKind.Utc).AddTicks(3489));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440004"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 21, 14, 20, 3, DateTimeKind.Utc).AddTicks(3500));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440005"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 21, 14, 20, 3, DateTimeKind.Utc).AddTicks(3507));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440006"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 21, 14, 20, 3, DateTimeKind.Utc).AddTicks(3512));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440007"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 21, 14, 20, 3, DateTimeKind.Utc).AddTicks(3518));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440008"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 21, 14, 20, 3, DateTimeKind.Utc).AddTicks(3523));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440009"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 21, 14, 20, 3, DateTimeKind.Utc).AddTicks(3528));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-44665544000a"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 21, 14, 20, 3, DateTimeKind.Utc).AddTicks(3533));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-44665544000b"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 21, 14, 20, 3, DateTimeKind.Utc).AddTicks(3538));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-44665544000c"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 21, 14, 20, 3, DateTimeKind.Utc).AddTicks(3543));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-44665544000d"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 21, 14, 20, 3, DateTimeKind.Utc).AddTicks(3548));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-44665544000e"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 21, 14, 20, 3, DateTimeKind.Utc).AddTicks(3553));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-44665544000f"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 21, 14, 20, 3, DateTimeKind.Utc).AddTicks(3558));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440010"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 21, 14, 20, 3, DateTimeKind.Utc).AddTicks(3563));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440011"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 21, 14, 20, 3, DateTimeKind.Utc).AddTicks(3568));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440012"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 21, 14, 20, 3, DateTimeKind.Utc).AddTicks(3573));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440013"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 21, 14, 20, 3, DateTimeKind.Utc).AddTicks(3578));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440014"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 21, 14, 20, 3, DateTimeKind.Utc).AddTicks(3583));
        }
    }
}
