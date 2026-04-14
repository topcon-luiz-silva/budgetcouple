using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetCouple.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationPreferencesAndHistory : Migration
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
                name: "notification_history",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    canal = table.Column<string>(type: "text", nullable: false),
                    tipo = table.Column<string>(type: "text", nullable: false),
                    titulo = table.Column<string>(type: "varchar(255)", nullable: false),
                    corpo = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    erro = table.Column<string>(type: "text", nullable: true),
                    enviado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notification_history", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "notification_preferences",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    email_habilitado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    email_endereco = table.Column<string>(type: "varchar(255)", nullable: true),
                    webpush_habilitado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    telegram_habilitado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    telegram_chat_id = table.Column<string>(type: "varchar(255)", nullable: true),
                    notificar_vencimentos_1_dia = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    notificar_vencimentos_dia = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    notificar_alertas_orcamento = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    notificar_faturas = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notification_preferences", x => x.id);
                });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "notification_history");

            migrationBuilder.DropTable(
                name: "notification_preferences");

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
    }
}
