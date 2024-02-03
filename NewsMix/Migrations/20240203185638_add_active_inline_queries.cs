using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewsMix.Migrations
{
    /// <inheritdoc />
    public partial class add_active_inline_queries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActiveInlineQueries",
                columns: table => new
                {
                    ExternalUserId = table.Column<string>(type: "TEXT", nullable: false),
                    QueryID = table.Column<string>(type: "TEXT", nullable: false),
                    CallbackActionType = table.Column<int>(type: "INTEGER", nullable: false),
                    Source = table.Column<string>(type: "TEXT", nullable: false),
                    TopicInternalName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.UpdateData(
                table: "NewsTopics",
                keyColumn: "Id",
                keyValue: 2,
                column: "VisibleNameRU",
                value: "Статьи с рейтингом > 25");

            migrationBuilder.UpdateData(
                table: "NewsTopics",
                keyColumn: "Id",
                keyValue: 3,
                column: "VisibleNameRU",
                value: "World of Warcraft");

            migrationBuilder.UpdateData(
                table: "NewsTopics",
                keyColumn: "Id",
                keyValue: 4,
                column: "VisibleNameRU",
                value: "World of Warcraft Classic");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActiveInlineQueries");

            migrationBuilder.UpdateData(
                table: "NewsTopics",
                keyColumn: "Id",
                keyValue: 2,
                column: "VisibleNameRU",
                value: "Рейтинг > 25");

            migrationBuilder.UpdateData(
                table: "NewsTopics",
                keyColumn: "Id",
                keyValue: 3,
                column: "VisibleNameRU",
                value: "World of warcraft");

            migrationBuilder.UpdateData(
                table: "NewsTopics",
                keyColumn: "Id",
                keyValue: 4,
                column: "VisibleNameRU",
                value: "World of warcraft Classic");
        }
    }
}
