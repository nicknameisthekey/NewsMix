using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewsMix.Migrations
{
    /// <inheritdoc />
    public partial class moved_hashtag_to_NewsTopics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HashTag",
                table: "FoundPublications");

            migrationBuilder.AddColumn<string>(
                name: "HashTag",
                table: "NewsTopics",
                type: "TEXT",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "NewsTopics",
                keyColumn: "Id",
                keyValue: 1,
                column: "HashTag",
                value: "#ea #apex");

            migrationBuilder.UpdateData(
                table: "NewsTopics",
                keyColumn: "Id",
                keyValue: 2,
                column: "HashTag",
                value: "#habr #rating25plus");

            migrationBuilder.UpdateData(
                table: "NewsTopics",
                keyColumn: "Id",
                keyValue: 3,
                column: "HashTag",
                value: "#icyveins #wow");

            migrationBuilder.UpdateData(
                table: "NewsTopics",
                keyColumn: "Id",
                keyValue: 4,
                column: "HashTag",
                value: "#icyveins #wowclassic");

            migrationBuilder.UpdateData(
                table: "NewsTopics",
                keyColumn: "Id",
                keyValue: 5,
                column: "HashTag",
                value: "#icyveins #diablo");

            migrationBuilder.UpdateData(
                table: "NewsTopics",
                keyColumn: "Id",
                keyValue: 6,
                column: "HashTag",
                value: "#icyveins #warcraft");

            migrationBuilder.UpdateData(
                table: "NewsTopics",
                keyColumn: "Id",
                keyValue: 7,
                column: "HashTag",
                value: "#icyveins #lostarc");

            migrationBuilder.UpdateData(
                table: "NewsTopics",
                keyColumn: "Id",
                keyValue: 8,
                column: "HashTag",
                value: "#noobclub #wow");

            migrationBuilder.UpdateData(
                table: "NewsTopics",
                keyColumn: "Id",
                keyValue: 9,
                column: "HashTag",
                value: "#noobclub #wowclassic");

            migrationBuilder.UpdateData(
                table: "NewsTopics",
                keyColumn: "Id",
                keyValue: 10,
                column: "HashTag",
                value: "#noobclub #diablo");

            migrationBuilder.UpdateData(
                table: "NewsTopics",
                keyColumn: "Id",
                keyValue: 11,
                column: "HashTag",
                value: "#noobclub #hearthstone");

            migrationBuilder.UpdateData(
                table: "NewsTopics",
                keyColumn: "Id",
                keyValue: 12,
                column: "HashTag",
                value: "#noobclub #overwatch");

            migrationBuilder.UpdateData(
                table: "NewsTopics",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "HashTag", "VisibleNameRU" },
                values: new object[] { "#noobclub #warcraft", "Warcraft" });

            migrationBuilder.UpdateData(
                table: "NewsTopics",
                keyColumn: "Id",
                keyValue: 14,
                column: "HashTag",
                value: "#noobclub #blizzard");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HashTag",
                table: "NewsTopics");

            migrationBuilder.AddColumn<string>(
                name: "HashTag",
                table: "FoundPublications",
                type: "TEXT",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "NewsTopics",
                keyColumn: "Id",
                keyValue: 13,
                column: "VisibleNameRU",
                value: "Warcraft 3");
        }
    }
}
