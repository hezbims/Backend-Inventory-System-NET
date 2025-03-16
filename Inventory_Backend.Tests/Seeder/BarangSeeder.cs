using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Database.Models;

namespace Inventory_Backend.Tests.Seeder;

public class BarangSeeder(MyDbContext dbContext) : IMySeeder
{
    public Barang Run(
        string name,
        string kodeBarang,
        Kategori kategori,
        int nomorRak,
        int nomorLaci,
        int nomorKolom,
        string uom = "Pc",
        int minStock = 10,
        int currentStock = 100,
        int lastMonthStock = 50,
        int unitPrice = 500000)
    {
        Barang barang = new Barang(
            nama: name,
            kodeBarang: kodeBarang,
            kategoriId: kategori.Id,
            nomorRak: nomorRak,
            nomorLaci: nomorLaci,
            nomorKolom: nomorKolom,
            uom: uom,
            minStock: minStock,
            currentStock: currentStock,
            lastMonthStock: lastMonthStock,
            unitPrice: unitPrice
        );

        dbContext.Barangs.Add(barang);
        dbContext.SaveChanges();
        
        return barang;
    }
}