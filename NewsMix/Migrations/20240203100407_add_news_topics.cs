using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NewsMix.Migrations
{
    /// <inheritdoc />
    public partial class add_news_topics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NewsTopics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NewsSource = table.Column<string>(type: "TEXT", nullable: false),
                    InternalName = table.Column<string>(type: "TEXT", nullable: false),
                    VisibleNameRU = table.Column<string>(type: "TEXT", nullable: false),
                    OrderInList = table.Column<byte>(type: "INTEGER", nullable: false),
                    Enabled = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsTopics", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "NewsTopics",
                columns: new[] { "Id", "Enabled", "InternalName", "NewsSource", "OrderInList", "VisibleNameRU" },
                values: new object[,]
                {
                    { 1, true, "apex", "ea", (byte)1, "Apex Legends" },
                    { 2, true, "r25+", "habr", (byte)1, "Рейтинг > 25" },
                    { 3, true, "wow", "icyveins", (byte)1, "World of warcraft" },
                    { 4, true, "wow-classic", "icyveins", (byte)2, "World of warcraft Classic" },
                    { 5, true, "diablo", "icyveins", (byte)3, "Diablo" },
                    { 6, true, "warcraft 3", "icyveins", (byte)4, "Warcraft" },
                    { 7, true, "lost ark", "icyveins", (byte)5, "Lost Arc" },
                    { 8, true, "wow", "noobclub", (byte)1, "World of Warcraft" },
                    { 9, true, "wow_classic", "noobclub", (byte)2, "World of Warcraft Classic" },
                    { 10, true, "diablo", "noobclub", (byte)3, "Diablo" },
                    { 11, true, "hearthstone", "noobclub", (byte)4, "Hearthstone" },
                    { 12, true, "overwatch", "noobclub", (byte)5, "Overwatch" },
                    { 13, true, "warcraft 3", "noobclub", (byte)6, "Warcraft 3" },
                    { 14, true, "blizzard", "noobclub", (byte)7, "Blizzard" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NewsTopics");
        }
    }
}
