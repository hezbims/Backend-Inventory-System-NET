using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory_Backend_NET.Migrations
{
    /// <inheritdoc />
    public partial class migrasi2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserTable",
                table: "UserTable");

            migrationBuilder.RenameTable(
                name: "UserTable",
                newName: "Users");

            migrationBuilder.RenameIndex(
                name: "IX_UserTable_Username",
                table: "Users",
                newName: "IX_Users_Username");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "UserTable");

            migrationBuilder.RenameIndex(
                name: "IX_Users_Username",
                table: "UserTable",
                newName: "IX_UserTable_Username");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserTable",
                table: "UserTable",
                column: "Id");
        }
    }
}
