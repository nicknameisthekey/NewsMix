using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewsMix.Migrations
{
    /// <inheritdoc />
    public partial class fix_subscription_fk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscription_Users_UserId",
                table: "Subscription");

            migrationBuilder.DropIndex(
                name: "IX_Subscription_UserId",
                table: "Subscription");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Subscription");

            migrationBuilder.CreateIndex(
                name: "IX_Subscription_InternalUserId",
                table: "Subscription",
                column: "InternalUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscription_Users_InternalUserId",
                table: "Subscription",
                column: "InternalUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscription_Users_InternalUserId",
                table: "Subscription");

            migrationBuilder.DropIndex(
                name: "IX_Subscription_InternalUserId",
                table: "Subscription");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Subscription",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Subscription_UserId",
                table: "Subscription",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscription_Users_UserId",
                table: "Subscription",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
