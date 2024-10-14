using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Seeder;

namespace Inventory_Backend.Tests.TestData;

public class BasicTestSeeder
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
}