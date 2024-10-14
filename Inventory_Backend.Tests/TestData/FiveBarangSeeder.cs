using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Database.Models;

namespace Inventory_Backend.Tests.TestData;

/// <summary>
/// Memasukkan 5 buah barang ke database
/// </summary>
public class FiveBarangSeeder
{
    private readonly MyDbContext _db;

    public FiveBarangSeeder(MyDbContext db)
    {
        _db = db;
    }
    
    public void Run()
    {
        var listKategori = _db.Kategoris.ToList();

        for (int i = 0; i < 5; i++)
        {
            _db.Barangs.Add(new Barang(
                nama: $"Barang {i + 1}",
                kategori: listKategori[i],
                kodeBarang: $"R1-1-{i + 1}",
                minStock: 1,
                nomorRak: 1,
                nomorKolom: 1,
                nomorLaci: i + 1,
                currentStock: 10,
                lastMonthStock: 10,
                unitPrice: 10000,
                uom: "Piece"
            ));
        }
        _db.SaveChanges();
    }
}