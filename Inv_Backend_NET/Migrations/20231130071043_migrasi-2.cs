using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Inventory_Backend_NET.Migrations
{
    /// <inheritdoc />
    public partial class migrasi2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Nama",
                table: "Barangs",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "KodeBarang",
                table: "Barangs",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Uom",
                table: "Barangs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "IsAdmin", "Password", "Username" },
                values: new object[,]
                {
                    { 1, true, "123", "admin" },
                    { 2, false, "123", "hezbi" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Barangs_KodeBarang",
                table: "Barangs",
                column: "KodeBarang",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Barangs_Nama",
                table: "Barangs",
                column: "Nama",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Barangs_NomorRak_NomorLaci_NomorKolom",
                table: "Barangs",
                columns: new[] { "NomorRak", "NomorLaci", "NomorKolom" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Barangs_KodeBarang",
                table: "Barangs");

            migrationBuilder.DropIndex(
                name: "IX_Barangs_Nama",
                table: "Barangs");

            migrationBuilder.DropIndex(
                name: "IX_Barangs_NomorRak_NomorLaci_NomorKolom",
                table: "Barangs");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DropColumn(
                name: "Uom",
                table: "Barangs");

            migrationBuilder.AlterColumn<string>(
                name: "Nama",
                table: "Barangs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "KodeBarang",
                table: "Barangs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
