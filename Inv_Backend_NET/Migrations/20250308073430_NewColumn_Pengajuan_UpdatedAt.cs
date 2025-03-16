using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory_Backend_NET.Migrations
{
    /// <inheritdoc />
    public partial class NewColumn_Pengajuan_UpdatedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "WaktuUpdate",
                table: "Pengajuans",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
            
            migrationBuilder.Sql("UPDATE Pengajuans SET WaktuUpdate = WaktuPengajuan");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WaktuUpdate",
                table: "Pengajuans");
        }
    }
}
