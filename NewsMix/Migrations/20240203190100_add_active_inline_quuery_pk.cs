using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewsMix.Migrations
{
    /// <inheritdoc />
    public partial class add_active_inline_quuery_pk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddPrimaryKey(
                name: "PK_ActiveInlineQueries",
                table: "ActiveInlineQueries",
                column: "QueryID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ActiveInlineQueries",
                table: "ActiveInlineQueries");
        }
    }
}
