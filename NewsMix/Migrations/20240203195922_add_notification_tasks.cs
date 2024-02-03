using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewsMix.Migrations
{
    /// <inheritdoc />
    public partial class add_notification_tasks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotifiedPublications");

            migrationBuilder.CreateTable(
                name: "FoundPublications",
                columns: table => new
                {
                    PublicationUrl = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAtUTC = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoundPublications", x => x.PublicationUrl);
                });

            migrationBuilder.CreateTable(
                name: "NotificationTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    InternalUserId = table.Column<int>(type: "INTEGER", nullable: false),
                    Url = table.Column<string>(type: "TEXT", nullable: false),
                    Topic = table.Column<string>(type: "TEXT", nullable: false),
                    HashTag = table.Column<string>(type: "TEXT", nullable: true),
                    Done = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAtUTC = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationTasks_Users_InternalUserId",
                        column: x => x.InternalUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NotificationTasks_InternalUserId",
                table: "NotificationTasks",
                column: "InternalUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FoundPublications");

            migrationBuilder.DropTable(
                name: "NotificationTasks");

            migrationBuilder.CreateTable(
                name: "NotifiedPublications",
                columns: table => new
                {
                    PublicationUniqeID = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotifiedPublications", x => x.PublicationUniqeID);
                });
        }
    }
}
