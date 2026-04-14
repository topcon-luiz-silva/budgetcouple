using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetCouple.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFaturaPagaToLancamento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AddColumn<bool>(
                name: "fatura_paga",
                table: "lancamentos",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "fatura_paga_em",
                table: "lancamentos",
                type: "timestamp with time zone",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "fatura_paga",
                table: "lancamentos");

            migrationBuilder.DropColumn(
                name: "fatura_paga_em",
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
                value: new DateTime(2026, 4, 14, 21, 13, 21, 487, DateTimeKind.Utc).AddTicks(4937));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440002"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 21, 13, 21, 487, DateTimeKind.Utc).AddTicks(4946));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440003"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 21, 13, 21, 487, DateTimeKind.Utc).AddTicks(4954));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440004"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 21, 13, 21, 487, DateTimeKind.Utc).AddTicks(4962));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440005"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 21, 13, 21, 487, DateTimeKind.Utc).AddTicks(4970));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440006"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 21, 13, 21, 487, DateTimeKind.Utc).AddTicks(4978));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440007"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 21, 13, 21, 487, DateTimeKind.Utc).AddTicks(4985));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440008"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 21, 13, 21, 487, DateTimeKind.Utc).AddTicks(4992));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440009"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 21, 13, 21, 487, DateTimeKind.Utc).AddTicks(4999));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-44665544000a"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 21, 13, 21, 487, DateTimeKind.Utc).AddTicks(5006));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-44665544000b"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 21, 13, 21, 487, DateTimeKind.Utc).AddTicks(5013));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-44665544000c"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 21, 13, 21, 487, DateTimeKind.Utc).AddTicks(5020));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-44665544000d"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 21, 13, 21, 487, DateTimeKind.Utc).AddTicks(5027));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-44665544000e"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 21, 13, 21, 487, DateTimeKind.Utc).AddTicks(5034));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-44665544000f"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 21, 13, 21, 487, DateTimeKind.Utc).AddTicks(5041));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440010"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 21, 13, 21, 487, DateTimeKind.Utc).AddTicks(5048));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440011"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 21, 13, 21, 487, DateTimeKind.Utc).AddTicks(5111));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440012"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 21, 13, 21, 487, DateTimeKind.Utc).AddTicks(5118));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440013"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 21, 13, 21, 487, DateTimeKind.Utc).AddTicks(5125));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440014"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 21, 13, 21, 487, DateTimeKind.Utc).AddTicks(5132));
        }
    }
}
