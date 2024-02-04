using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewsMix.Migrations
{
    /// <inheritdoc />
    public partial class found_publications_countains_more_data : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HashTag",
                table: "FoundPublications",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Source",
                table: "FoundPublications",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TopicInternalName",
                table: "FoundPublications",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HashTag",
                table: "FoundPublications");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "FoundPublications");

            migrationBuilder.DropColumn(
                name: "TopicInternalName",
                table: "FoundPublications");
        }
    }
}
