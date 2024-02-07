using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NewsMix.Migrations
{
    /// <inheritdoc />
    public partial class add_wowhead : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "NewsTopics",
                columns: new[] { "Id", "Enabled", "HashTag", "InternalName", "NewsSource", "OrderInList", "VisibleNameRU" },
                values: new object[,]
                {
                    { 15, true, "#wowhead #wow", "wow", "wowhead", (byte)1, "World of Warcraft" },
                    { 16, true, "#wowhead #wowclassic", "wow_classic", "wowhead", (byte)2, "World of Warcraft Classic" },
                    { 17, true, "#wowhead #wowclassicseries", "wow_classic_series", "wowhead", (byte)3, "World of Warcraft Classic Series" },
                    { 18, true, "#wowhead #wowclassic #wowwotlk", "wotlk", "wowhead", (byte)4, "World of Warcraft WOTLK" },
                    { 19, true, "#wowhead #wowclassic #wowcata", "cata", "wowhead", (byte)5, "World of Warcraft Cataclysm" },
                    { 20, true, "#wowhead #diablo", "diablo", "wowhead", (byte)6, "Diablo" },
                    { 21, true, "#wowhead #diablo #diablo4", "diablo-4", "wowhead", (byte)7, "Diablo 4" },
                    { 22, true, "#wowhead #diablo #diabloimmortal", "diablo-immortal", "wowhead", (byte)8, "Diablo Immortal" },
                    { 23, true, "#wowhead #indev", "in-development", "wowhead", (byte)9, "Games in development" },
                    { 24, true, "#wowhead #other", "other", "wowhead", (byte)10, "Other" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "NewsTopics",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "NewsTopics",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "NewsTopics",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "NewsTopics",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "NewsTopics",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "NewsTopics",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "NewsTopics",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "NewsTopics",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "NewsTopics",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "NewsTopics",
                keyColumn: "Id",
                keyValue: 24);
        }
    }
}
