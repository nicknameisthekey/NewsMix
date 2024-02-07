using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewsMix.Migrations
{
    /// <inheritdoc />
    public partial class add_bot_sent_messages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Done",
                table: "NotificationTasks");

            migrationBuilder.AddColumn<DateTime>(
                name: "DoneAtUTC",
                table: "NotificationTasks",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BotSentMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ExternalUserId = table.Column<string>(type: "TEXT", nullable: false),
                    TelegramMessageId = table.Column<long>(type: "INTEGER", nullable: false),
                    MessageType = table.Column<int>(type: "INTEGER", nullable: false),
                    SendAtUTC = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DeletedAtUTC = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BotSentMessages", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BotSentMessages");

            migrationBuilder.DropColumn(
                name: "DoneAtUTC",
                table: "NotificationTasks");

            migrationBuilder.AddColumn<bool>(
                name: "Done",
                table: "NotificationTasks",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }
    }
}
