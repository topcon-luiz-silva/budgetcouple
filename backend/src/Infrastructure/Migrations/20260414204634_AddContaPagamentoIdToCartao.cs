using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetCouple.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddContaPagamentoIdToCartao : Migration
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

            migrationBuilder.AddColumn<Guid>(
                name: "conta_pagamento_id",
                table: "cartoes",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

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
                value: new DateTime(2026, 4, 14, 20, 46, 33, 989, DateTimeKind.Utc).AddTicks(5443));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440002"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 20, 46, 33, 989, DateTimeKind.Utc).AddTicks(5452));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440003"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 20, 46, 33, 989, DateTimeKind.Utc).AddTicks(5459));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440004"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 20, 46, 33, 989, DateTimeKind.Utc).AddTicks(5465));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440005"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 20, 46, 33, 989, DateTimeKind.Utc).AddTicks(5471));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440006"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 20, 46, 33, 989, DateTimeKind.Utc).AddTicks(5478));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440007"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 20, 46, 33, 989, DateTimeKind.Utc).AddTicks(5484));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440008"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 20, 46, 33, 989, DateTimeKind.Utc).AddTicks(5491));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440009"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 20, 46, 33, 989, DateTimeKind.Utc).AddTicks(5497));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-44665544000a"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 20, 46, 33, 989, DateTimeKind.Utc).AddTicks(5505));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-44665544000b"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 20, 46, 33, 989, DateTimeKind.Utc).AddTicks(5511));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-44665544000c"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 20, 46, 33, 989, DateTimeKind.Utc).AddTicks(5517));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-44665544000d"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 20, 46, 33, 989, DateTimeKind.Utc).AddTicks(5523));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-44665544000e"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 20, 46, 33, 989, DateTimeKind.Utc).AddTicks(5529));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-44665544000f"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 20, 46, 33, 989, DateTimeKind.Utc).AddTicks(5535));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440010"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 20, 46, 33, 989, DateTimeKind.Utc).AddTicks(5541));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440011"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 20, 46, 33, 989, DateTimeKind.Utc).AddTicks(5547));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440012"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 20, 46, 33, 989, DateTimeKind.Utc).AddTicks(5553));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440013"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 20, 46, 33, 989, DateTimeKind.Utc).AddTicks(5559));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440014"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 20, 46, 33, 989, DateTimeKind.Utc).AddTicks(5565));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "conta_pagamento_id",
                table: "cartoes");

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
                value: new DateTime(2026, 4, 14, 20, 22, 32, 223, DateTimeKind.Utc).AddTicks(5792));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440002"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 20, 22, 32, 223, DateTimeKind.Utc).AddTicks(5820));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440003"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 20, 22, 32, 223, DateTimeKind.Utc).AddTicks(5840));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440004"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 20, 22, 32, 223, DateTimeKind.Utc).AddTicks(5858));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440005"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 20, 22, 32, 223, DateTimeKind.Utc).AddTicks(5875));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440006"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 20, 22, 32, 223, DateTimeKind.Utc).AddTicks(5893));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440007"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 20, 22, 32, 223, DateTimeKind.Utc).AddTicks(5910));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440008"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 20, 22, 32, 223, DateTimeKind.Utc).AddTicks(5927));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440009"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 20, 22, 32, 223, DateTimeKind.Utc).AddTicks(5944));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-44665544000a"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 20, 22, 32, 223, DateTimeKind.Utc).AddTicks(5961));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-44665544000b"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 20, 22, 32, 223, DateTimeKind.Utc).AddTicks(5978));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-44665544000c"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 20, 22, 32, 223, DateTimeKind.Utc).AddTicks(6001));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-44665544000d"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 20, 22, 32, 223, DateTimeKind.Utc).AddTicks(6017));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-44665544000e"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 20, 22, 32, 223, DateTimeKind.Utc).AddTicks(6035));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-44665544000f"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 20, 22, 32, 223, DateTimeKind.Utc).AddTicks(6052));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440010"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 20, 22, 32, 223, DateTimeKind.Utc).AddTicks(6069));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440011"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 20, 22, 32, 223, DateTimeKind.Utc).AddTicks(6085));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440012"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 20, 22, 32, 223, DateTimeKind.Utc).AddTicks(6102));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440013"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 20, 22, 32, 223, DateTimeKind.Utc).AddTicks(6119));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440014"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 20, 22, 32, 223, DateTimeKind.Utc).AddTicks(6136));
        }
    }
}
