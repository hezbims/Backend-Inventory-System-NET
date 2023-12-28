using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Database.Models;

namespace Inventory_Backend.Tests.TestConfiguration.Seeder;

public static class Barang20WithQuantity20
{
    public static List<Barang> SeedBarang20WithQuantity20(
        this MyDbContext db
    )
    {
        var kategoris = Enumerable.Range(1, 5).Select(
            index =>
            {
                var kategori = new Kategori(
                    nama: $"NamaKategori-{index}"
                );
                db.Add(kategori);
                return kategori;
            }
        ).ToList();

        var barangs = Enumerable.Range(0, 20).Select(
            i =>
            {
                var barang = new Barang(
                    nama: $"Barang-{i}",
                    kodeBarang: i.ToString(),
                    kategori: kategoris[i % kategoris.Count],
                    minStock: 0,
                    nomorRak: 1,
                    nomorKolom: i / 9 + 1,
                    nomorLaci: i % 9 + 1,
                    currentStock: 20,
                    lastMonthStock: 1,
                    unitPrice: 1,
                    uom: "pc"
                );
                db.Barangs.Add(barang);
                return barang;
            }
        ).ToList();

        db.SaveChanges();
        return db.Barangs.ToList();
    }
}