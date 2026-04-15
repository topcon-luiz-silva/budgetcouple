using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetCouple.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixCategoriaSeedData : Migration
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

            migrationBuilder.AlterColumn<List<string>>(
                name: "telegram_chat_ids",
                table: "app_config",
                type: "text[]",
                nullable: false,
                defaultValue: new List<string>(),
                oldClrType: typeof(List<string>),
                oldType: "text[]",
                oldDefaultValue: new List<string>());

            migrationBuilder.CreateTable(
                name: "lancamento_anexos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    lancamento_id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome_arquivo = table.Column<string>(type: "varchar(255)", nullable: false),
                    content_type = table.Column<string>(type: "varchar(100)", nullable: false),
                    tamanho_bytes = table.Column<long>(type: "bigint", nullable: false),
                    caminho_storage = table.Column<string>(type: "varchar(500)", nullable: false),
                    enviado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lancamento_anexos", x => x.id);
                    table.ForeignKey(
                        name: "FK_lancamento_anexos_lancamentos_lancamento_id",
                        column: x => x.lancamento_id,
                        principalTable: "lancamentos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440001"),
                column: "criado_em",
                value: new DateTime(2026, 4, 15, 11, 50, 8, 310, DateTimeKind.Utc).AddTicks(7587));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440002"),
                column: "criado_em",
                value: new DateTime(2026, 4, 15, 11, 50, 8, 310, DateTimeKind.Utc).AddTicks(7633));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440003"),
                column: "criado_em",
                value: new DateTime(2026, 4, 15, 11, 50, 8, 310, DateTimeKind.Utc).AddTicks(7664));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440004"),
                column: "criado_em",
                value: new DateTime(2026, 4, 15, 11, 50, 8, 310, DateTimeKind.Utc).AddTicks(7692));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440005"),
                column: "criado_em",
                value: new DateTime(2026, 4, 15, 11, 50, 8, 310, DateTimeKind.Utc).AddTicks(7719));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440006"),
                column: "criado_em",
                value: new DateTime(2026, 4, 15, 11, 50, 8, 310, DateTimeKind.Utc).AddTicks(7745));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440007"),
                column: "criado_em",
                value: new DateTime(2026, 4, 15, 11, 50, 8, 310, DateTimeKind.Utc).AddTicks(7775));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440008"),
                column: "criado_em",
                value: new DateTime(2026, 4, 15, 11, 50, 8, 310, DateTimeKind.Utc).AddTicks(7801));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440009"),
                column: "criado_em",
                value: new DateTime(2026, 4, 15, 11, 50, 8, 310, DateTimeKind.Utc).AddTicks(7830));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-44665544000a"),
                column: "criado_em",
                value: new DateTime(2026, 4, 15, 11, 50, 8, 310, DateTimeKind.Utc).AddTicks(7857));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-44665544000b"),
                column: "criado_em",
                value: new DateTime(2026, 4, 15, 11, 50, 8, 310, DateTimeKind.Utc).AddTicks(7883));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-44665544000c"),
                column: "criado_em",
                value: new DateTime(2026, 4, 15, 11, 50, 8, 310, DateTimeKind.Utc).AddTicks(7909));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-44665544000d"),
                column: "criado_em",
                value: new DateTime(2026, 4, 15, 11, 50, 8, 310, DateTimeKind.Utc).AddTicks(7935));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-44665544000e"),
                column: "criado_em",
                value: new DateTime(2026, 4, 15, 11, 50, 8, 310, DateTimeKind.Utc).AddTicks(7961));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-44665544000f"),
                column: "criado_em",
                value: new DateTime(2026, 4, 15, 11, 50, 8, 310, DateTimeKind.Utc).AddTicks(7987));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440010"),
                column: "criado_em",
                value: new DateTime(2026, 4, 15, 11, 50, 8, 310, DateTimeKind.Utc).AddTicks(8013));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440011"),
                column: "criado_em",
                value: new DateTime(2026, 4, 15, 11, 50, 8, 310, DateTimeKind.Utc).AddTicks(8038));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440012"),
                column: "criado_em",
                value: new DateTime(2026, 4, 15, 11, 50, 8, 310, DateTimeKind.Utc).AddTicks(8064));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440013"),
                column: "criado_em",
                value: new DateTime(2026, 4, 15, 11, 50, 8, 310, DateTimeKind.Utc).AddTicks(8090));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440014"),
                column: "criado_em",
                value: new DateTime(2026, 4, 15, 11, 50, 8, 310, DateTimeKind.Utc).AddTicks(8116));

            migrationBuilder.CreateIndex(
                name: "ix_lanc_anexo_lancamento_id",
                table: "lancamento_anexos",
                column: "lancamento_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "lancamento_anexos");

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
                value: new DateTime(2026, 4, 14, 22, 10, 10, 843, DateTimeKind.Utc).AddTicks(4641));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440002"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 22, 10, 10, 843, DateTimeKind.Utc).AddTicks(4654));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440003"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 22, 10, 10, 843, DateTimeKind.Utc).AddTicks(4663));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440004"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 22, 10, 10, 843, DateTimeKind.Utc).AddTicks(4672));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440005"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 22, 10, 10, 843, DateTimeKind.Utc).AddTicks(4680));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440006"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 22, 10, 10, 843, DateTimeKind.Utc).AddTicks(4689));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440007"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 22, 10, 10, 843, DateTimeKind.Utc).AddTicks(4698));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440008"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 22, 10, 10, 843, DateTimeKind.Utc).AddTicks(4706));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440009"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 22, 10, 10, 843, DateTimeKind.Utc).AddTicks(4715));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-44665544000a"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 22, 10, 10, 843, DateTimeKind.Utc).AddTicks(4723));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-44665544000b"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 22, 10, 10, 843, DateTimeKind.Utc).AddTicks(4731));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-44665544000c"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 22, 10, 10, 843, DateTimeKind.Utc).AddTicks(4739));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-44665544000d"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 22, 10, 10, 843, DateTimeKind.Utc).AddTicks(4747));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-44665544000e"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 22, 10, 10, 843, DateTimeKind.Utc).AddTicks(4755));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-44665544000f"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 22, 10, 10, 843, DateTimeKind.Utc).AddTicks(4763));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440010"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 22, 10, 10, 843, DateTimeKind.Utc).AddTicks(4771));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440011"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 22, 10, 10, 843, DateTimeKind.Utc).AddTicks(4779));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440012"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 22, 10, 10, 843, DateTimeKind.Utc).AddTicks(4787));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440013"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 22, 10, 10, 843, DateTimeKind.Utc).AddTicks(4940));

            migrationBuilder.UpdateData(
                table: "categorias",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440014"),
                column: "criado_em",
                value: new DateTime(2026, 4, 14, 22, 10, 10, 843, DateTimeKind.Utc).AddTicks(4949));
        }
    }
}
