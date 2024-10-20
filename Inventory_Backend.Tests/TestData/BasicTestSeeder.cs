using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Database.Models;
using Inventory_Backend_NET.Seeder;

namespace Inventory_Backend.Tests.TestData;

/// <summary>
/// Seeder yang akan mengisi database dengan :
/// <ul>
/// <li>5 Barang</li>
/// <li>10 Kategori</li>
/// <li>3 User</li>
/// <li>10 Pengaju (5 Grup, 5 Pemasok)</li>
/// </ul>
/// </summary>
public class BasicTestSeeder : IDisposable
{
    private readonly MyDbContext _db;
    private readonly FiveBarangSeeder _fiveBarangSeeder;
    private readonly TenKategoriSeeder _tenKategoriSeeder;
    private readonly ThreeUserSeeder _threeUserSeeder;
    private readonly TenPengajuSeeder _tenPengajuSeeder;
    
    public BasicTestSeeder(MyDbContext db)
    {
        _db = db;
        _fiveBarangSeeder = new FiveBarangSeeder(db);
        _tenKategoriSeeder = new TenKategoriSeeder(db);
        _threeUserSeeder = new ThreeUserSeeder(db);
        _tenPengajuSeeder = new TenPengajuSeeder(db);
    }
    
    public BasicTestData Run()
    {
        using var transaction = _db.Database.BeginTransaction();
        _tenKategoriSeeder.Run();
        _threeUserSeeder.Run();
        _tenPengajuSeeder.Run();
        _fiveBarangSeeder.Run();
        
        transaction.Commit();

        return BasicTestData.CreateFrom(db: _db);
    }

    public void Dispose()
    {
        _db.Dispose();
    }
}

public record BasicTestData
{
    public required User Admin { get; init; }
    public required User NonAdmin { get; init; }
    public required List<Barang> ListBarang { get; init; }
    public required Pengaju Grup { get; init; }
    public required Pengaju Pemasok { get; init; }

    public static BasicTestData CreateFrom(MyDbContext db)
    {
        return new BasicTestData
        {
            Admin = db.Users.First(user => user.IsAdmin),
            NonAdmin = db.Users.First(user => !user.IsAdmin),
            ListBarang = db.Barangs.ToList(),
            Grup = db.Pengajus.First(pengaju => !pengaju.IsPemasok),
            Pemasok = db.Pengajus.First(pengaju => pengaju.IsPemasok)
        };
    }
} 