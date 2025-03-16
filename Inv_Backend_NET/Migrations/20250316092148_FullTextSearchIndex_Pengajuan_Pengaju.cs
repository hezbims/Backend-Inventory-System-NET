using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory_Backend_NET.Migrations
{
    /// <inheritdoc />
    public partial class FullTextSearchIndex_Pengajuan_Pengaju : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Membuat full text index pada Pengaju-Nama dan juga Pengajuan-KodeTransaksi
            migrationBuilder.Sql(@"
                -- Step 1: Create Full-Text Catalog (if not exists)
                IF NOT EXISTS (SELECT * FROM sys.fulltext_catalogs WHERE name = 'Inventory_FullTextCatalog')
                CREATE FULLTEXT CATALOG Inventory_FullTextCatalog AS DEFAULT;
                
                -- Step 2: Ensure a Unique Index (Required for Full-Text Index)
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Pengajuans_KodeTransaksi')
                CREATE UNIQUE INDEX IX_Pengajuans_KodeTransaksi ON Pengajuans(KodeTransaksi);

                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Pengajus_Nama')
                CREATE UNIQUE INDEX IX_Pengajus_Nama ON Pengajus(Nama);
                
                -- Step 3: Create Full-Text Index
                CREATE FULLTEXT INDEX ON Pengajuans(KodeTransaksi)
                KEY INDEX IX_Pengajuans_KodeTransaksi
                ON Inventory_FullTextCatalog
                WITH CHANGE_TRACKING AUTO;

                CREATE FULLTEXT INDEX ON Pengajus(Nama)
                KEY INDEX IX_Pengajus_Nama
                ON Inventory_FullTextCatalog
                WITH CHANGE_TRACKING AUTO;
            ", true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                -- Drop Full-Text Index if it exists
                IF EXISTS (SELECT * FROM sys.fulltext_indexes WHERE object_id = OBJECT_ID('Pengajus'))
                DROP FULLTEXT INDEX ON Pengajus;

                IF EXISTS (SELECT * FROM sys.fulltext_indexes WHERE object_id = OBJECT_ID('Pengajuans'))
                DROP FULLTEXT INDEX ON Pengajuans;
            ");
        }
    }
}
