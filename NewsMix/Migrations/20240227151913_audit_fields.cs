using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewsMix.Migrations
{
    /// <inheritdoc />
    public partial class audit_fields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscription_Users_InternalUserId",
                table: "Subscription");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Subscription",
                table: "Subscription");

            migrationBuilder.RenameTable(
                name: "Subscription",
                newName: "Subscriptions");

            migrationBuilder.RenameColumn(
                name: "Topic",
                table: "NotificationTasks",
                newName: "TopicInternalName");

            migrationBuilder.RenameIndex(
                name: "IX_Subscription_InternalUserId",
                table: "Subscriptions",
                newName: "IX_Subscriptions_InternalUserId");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "NotificationTasks",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<string>(
                name: "NewsSource",
                table: "NotificationTasks",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "NotificationTaskId",
                table: "BotSentMessages",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Subscriptions",
                table: "Subscriptions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Users_InternalUserId",
                table: "Subscriptions",
                column: "InternalUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Users_InternalUserId",
                table: "Subscriptions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Subscriptions",
                table: "Subscriptions");

            migrationBuilder.DropColumn(
                name: "NewsSource",
                table: "NotificationTasks");

            migrationBuilder.DropColumn(
                name: "NotificationTaskId",
                table: "BotSentMessages");

            migrationBuilder.RenameTable(
                name: "Subscriptions",
                newName: "Subscription");

            migrationBuilder.RenameColumn(
                name: "TopicInternalName",
                table: "NotificationTasks",
                newName: "Topic");

            migrationBuilder.RenameIndex(
                name: "IX_Subscriptions_InternalUserId",
                table: "Subscription",
                newName: "IX_Subscription_InternalUserId");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "NotificationTasks",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Subscription",
                table: "Subscription",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscription_Users_InternalUserId",
                table: "Subscription",
                column: "InternalUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
