using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Inventory_Backend_NET.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Kategoris",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nama = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kategoris", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pengajus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nama = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsPemasok = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pengajus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StatusPengajuans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusPengajuans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TotalPengajuanByTanggals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tanggal = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Total = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TotalPengajuanByTanggals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsAdmin = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Barangs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<long>(type: "bigint", nullable: false),
                    KodeBarang = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Nama = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    KategoriId = table.Column<int>(type: "int", nullable: false),
                    MinStock = table.Column<int>(type: "int", nullable: false),
                    NomorRak = table.Column<int>(type: "int", nullable: false),
                    NomorLaci = table.Column<int>(type: "int", nullable: false),
                    NomorKolom = table.Column<int>(type: "int", nullable: false),
                    CurrentStock = table.Column<int>(type: "int", nullable: false),
                    LastMonthStock = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<int>(type: "int", nullable: false),
                    Uom = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Barangs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Barangs_Kategoris_KategoriId",
                        column: x => x.KategoriId,
                        principalTable: "Kategoris",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pengajuans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KodeTransaksi = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    WaktuPengajuan = table.Column<long>(type: "bigint", nullable: false),
                    PengajuId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pengajuans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pengajuans_Pengajus_PengajuId",
                        column: x => x.PengajuId,
                        principalTable: "Pengajus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Pengajuans_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BarangAjuans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PengajuanId = table.Column<int>(type: "int", nullable: false),
                    BarangId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Keterangan = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BarangAjuans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BarangAjuans_Barangs_BarangId",
                        column: x => x.BarangId,
                        principalTable: "Barangs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BarangAjuans_Pengajuans_PengajuanId",
                        column: x => x.PengajuanId,
                        principalTable: "Pengajuans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "StatusPengajuans",
                columns: new[] { "Id", "Value" },
                values: new object[,]
                {
                    { 1, "diterima" },
                    { 2, "menunggu" },
                    { 3, "ditolak" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BarangAjuans_BarangId",
                table: "BarangAjuans",
                column: "BarangId");

            migrationBuilder.CreateIndex(
                name: "IX_BarangAjuans_PengajuanId",
                table: "BarangAjuans",
                column: "PengajuanId");

            migrationBuilder.CreateIndex(
                name: "IX_Barangs_KategoriId",
                table: "Barangs",
                column: "KategoriId");

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

            migrationBuilder.CreateIndex(
                name: "IX_Kategoris_Nama",
                table: "Kategoris",
                column: "Nama",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pengajuans_KodeTransaksi",
                table: "Pengajuans",
                column: "KodeTransaksi",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pengajuans_PengajuId",
                table: "Pengajuans",
                column: "PengajuId");

            migrationBuilder.CreateIndex(
                name: "IX_Pengajuans_UserId",
                table: "Pengajuans",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Pengajus_Nama",
                table: "Pengajus",
                column: "Nama",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BarangAjuans");

            migrationBuilder.DropTable(
                name: "StatusPengajuans");

            migrationBuilder.DropTable(
                name: "TotalPengajuanByTanggals");

            migrationBuilder.DropTable(
                name: "Barangs");

            migrationBuilder.DropTable(
                name: "Pengajuans");

            migrationBuilder.DropTable(
                name: "Kategoris");

            migrationBuilder.DropTable(
                name: "Pengajus");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
