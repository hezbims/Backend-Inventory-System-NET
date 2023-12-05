using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Inventory_Backend_NET.Models;
using Inventory_Backend_NET.Seeder.Data;
using NeoSmart.Caching.Sqlite;

namespace Inventory_Backend_NET.Seeder;

public static class TestSeederExtension
{
    public static void TestSeeder(this IServiceProvider serviceProvider)
    {
        var db = serviceProvider.GetRequiredService<MyDbContext>();
        var sqliteCache = serviceProvider.GetRequiredService<SqliteCache>();
        
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            PrepareHeaderForMatch = args => args.Header.ToUpper(),
        };
        
        using (var reader = new StreamReader("Seeder/Data/barang_seeder.csv"))
        using (var csv = new CsvReader(reader, config))
        {
            var rand = new Random(5);

            for (var i = 1 ; i <= 10 ; i++)
            {
                var kategori = new Kategori { Nama = $"Kategori {i}" };
                db.Kategoris.Add(kategori);
            }

            db.SaveChanges();
            
            var barangs = csv
                .GetRecords<BarangCsvDto>()
                .ToList()
                .Select(
                    barangCsvDto =>
                    {
                        var rak = barangCsvDto.Rak;
                        var namaKategori = $"Kategori {rand.Next() % 10 + 1}";
                        var barang = new Barang
                        {
                            KodeBarang = $"R{rak.NomorRak}-{rak.NomorLaci}-{rak.NomorKolom}",
                            Nama = barangCsvDto.ItemDescription,
                            Kategori = db.Kategoris.Where(kategori =>
                                kategori.Nama == namaKategori
                            ).First(),
                            MinStock = barangCsvDto.MinStock,
                            NomorRak = rak.NomorRak,
                            NomorLaci = rak.NomorLaci,
                            NomorKolom = rak.NomorKolom,
                            CurrentStock = barangCsvDto.Actual ?? 0,
                            LastMonthStock = barangCsvDto.LastMonthStock ?? 0,
                            UnitPrice = barangCsvDto.IntUnitPrice,
                            Uom = barangCsvDto.Uom
                        };
                        db.Barangs.Add(barang);
                        return barang;
                    }    
                )
                .ToList();

            db.Pengajuans.Add(
                new Pengajuan(sqliteCache)
                {
                    Pengaju = new Pengaju
                    {
                        Nama = "Grup 1",
                        IsPemasok = false
                    },
                    Status = StatusPengajuan.Diterima,
                    User = new User
                    {
                        Username = "hezbi",
                        Password = "123",
                        IsAdmin = false,
                    },
                    BarangAjuans = new List<BarangAjuan>
                    {
                        new BarangAjuan
                        {
                            Barang = barangs[rand.Next() % barangs.Count],
                            Quantity = 1,
                            Keterangan = null
                        }
                    }
                }
            );

            db.SaveChanges();
        }
    }
}