using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Database.Models;

namespace Inventory_Backend_NET.Seeder;

/// <summary>
/// Seeder yang akan mengisi database dengan 5 pemasok dan 5 group
/// </summary>
public class TenPengajuSeeder
{
    private readonly MyDbContext _db;

    public TenPengajuSeeder(MyDbContext db)
    {
        _db = db;
    }

    public List<Pengaju> Run()
    {
        var listPengaju = new List<Pengaju>();
        for (int i = 1 ; i <= 5; i++)
        {
            var grup = new Pengaju(
                nama : $"Grup {i}",
                isPemasok : false
            );
            listPengaju.Add(grup);
            _db.Pengajus.Add(grup);
        }
        for (int i = 1 ; i <= 3; i++)
        {
            var pemasok = new Pengaju(
                nama : $"Pemasok {i}",
                isPemasok : true
            );
            listPengaju.Add(pemasok);
            _db.Pengajus.Add(pemasok);
        }

        _db.SaveChanges();
        return listPengaju;
    }
}