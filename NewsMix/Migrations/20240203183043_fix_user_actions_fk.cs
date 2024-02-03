using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewsMix.Migrations
{
    /// <inheritdoc />
    public partial class fix_user_actions_fk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserActions_Users_UserId",
                table: "UserActions");

            migrationBuilder.DropIndex(
                name: "IX_UserActions_UserId",
                table: "UserActions");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "UserActions");

            migrationBuilder.CreateIndex(
                name: "IX_UserActions_InternalUserId",
                table: "UserActions",
                column: "InternalUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserActions_Users_InternalUserId",
                table: "UserActions",
                column: "InternalUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserActions_Users_InternalUserId",
                table: "UserActions");

            migrationBuilder.DropIndex(
                name: "IX_UserActions_InternalUserId",
                table: "UserActions");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "UserActions",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserActions_UserId",
                table: "UserActions",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserActions_Users_UserId",
                table: "UserActions",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
