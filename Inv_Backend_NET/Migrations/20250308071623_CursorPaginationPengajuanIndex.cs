using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory_Backend_NET.Migrations
{
    /// <inheritdoc />
    public partial class CursorPaginationPengajuanIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Pengajuans_WaktuPengajuan_Id",
                table: "Pengajuans",
                columns: new[] { "WaktuPengajuan", "Id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Pengajuans_WaktuPengajuan_Id",
                table: "Pengajuans");
        }
    }
}
