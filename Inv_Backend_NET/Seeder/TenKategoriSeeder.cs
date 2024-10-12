using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Database.Models;

namespace Inventory_Backend_NET.Seeder;

/// <summary>
/// Membuat 10 kategori baru pada database
/// </summary>
public class TenKategoriSeeder
{
    private readonly MyDbContext _db;
    public TenKategoriSeeder(MyDbContext db)
    {
        _db = db;
    }

    public List<Kategori> Run()
    {
        var listKategori = Enumerable.Range(1, 10).Select(i =>
            {
                var kategori = new Kategori(nama : $"NamaKategori {i}");
                _db.Kategoris.Add(kategori);
                return kategori;
            }
        ).ToList();
        _db.SaveChanges();

        return listKategori;
    }
}