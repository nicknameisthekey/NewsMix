using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewsMix.Migrations
{
    /// <inheritdoc />
    public partial class add_user_ids : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Users",
                newName: "ExternalUserId");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "UserActions",
                newName: "CreatedAtUTC");

            migrationBuilder.RenameColumn(
                name: "Topic",
                table: "Subscription",
                newName: "TopicInternalName");

            migrationBuilder.AddColumn<int>(
                name: "InternalUserId",
                table: "UserActions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOnUTC",
                table: "Subscription",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "InternalUserId",
                table: "Subscription",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InternalUserId",
                table: "UserActions");

            migrationBuilder.DropColumn(
                name: "CreatedOnUTC",
                table: "Subscription");

            migrationBuilder.DropColumn(
                name: "InternalUserId",
                table: "Subscription");

            migrationBuilder.RenameColumn(
                name: "ExternalUserId",
                table: "Users",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "CreatedAtUTC",
                table: "UserActions",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "TopicInternalName",
                table: "Subscription",
                newName: "Topic");
        }
    }
}
