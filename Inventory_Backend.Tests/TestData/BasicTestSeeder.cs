using Inventory_Backend_NET.Database;
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
    
    public void Run()
    {
        using var transaction = _db.Database.BeginTransaction();
        _tenKategoriSeeder.Run();
        _threeUserSeeder.Run();
        _tenPengajuSeeder.Run();
        _fiveBarangSeeder.Run();
        
        transaction.Commit();
    }

    public void Dispose()
    {
        _db.Dispose();
    }
}