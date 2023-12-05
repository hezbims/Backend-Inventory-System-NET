using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Inventory_Backend_NET.Models;
using Inventory_Backend_NET.Seeder.Data;

namespace Inventory_Backend_NET.Seeder;

public static class TestSeederExtension
{
    public static void TestSeeder(this MyDbContext db)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            PrepareHeaderForMatch = args => args.Header.ToUpper(),
        };
        
        using (var reader = new StreamReader("Seeder/Data/barang_seeder.csv"))
        using (var csv = new CsvReader(reader, config))
        {
            var rand = new Random(5);
            var barangs = csv.GetRecords<BarangCsvDto>().ToList();
            for (var i = 1; i <= 10; ++i)
            {
                Console.WriteLine();
                db.Kategoris.Add(new Kategori { Nama = $"Kategori {i}" });
            }

            db.SaveChanges();

            foreach (var barang in barangs)
            {
                var rak = barang.Rak;
                var namaKategori = $"Kategori {rand.Next() % 10 + 1}";
                db.Barangs.Add(new Barang
                {
                    KodeBarang = $"R{rak.NomorRak}-{rak.NomorLaci}-{rak.NomorKolom}",
                    Nama = barang.ItemDescription,
                    Kategori = db.Kategoris.Where(kategori =>
                        kategori.Nama == namaKategori
                    ).First(),
                    MinStock = barang.MinStock,
                    NomorRak = rak.NomorRak,
                    NomorLaci = rak.NomorLaci,
                    NomorKolom = rak.NomorKolom,
                    CurrentStock = barang.Actual ?? 0,
                    LastMonthStock = barang.LastMonthStock ?? 0,
                    UnitPrice = barang.IntUnitPrice,
                    Uom = barang.Uom
                });
            }

            db.Pengajuans.Add(
                new Pengajuan
                {
                    Pengaju = new Pengaju
                    {
                        Nama = "Grup 1",
                        IsPemasok = false
                    },
                    UrutanHariIni = 
                }
            );

            db.SaveChanges();
        }
    }
}