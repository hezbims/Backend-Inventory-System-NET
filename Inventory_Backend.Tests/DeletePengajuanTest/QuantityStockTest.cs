using Inventory_Backend_NET.Database.Models;
using Inventory_Backend_NET.Fitur.Pengajuan.DeletePengajuan;
using Inventory_Backend.Tests.TestConfiguration.Constant;
using Inventory_Backend.Tests.TestConfiguration.Fixture;
using Inventory_Backend.Tests.TestConfiguration.Mock;
using Inventory_Backend.Tests.TestConfiguration.Seeder;
using Microsoft.AspNetCore.Mvc;
using NeoSmart.Caching.Sqlite;

namespace Inventory_Backend.Tests.DeletePengajuanTest;

[Collection(TestConstant.WithDbCollection)]
public class QuantityStockTest : IDisposable
{
    private readonly MyDbFixture _fixture;
    private readonly User _admin;
    private readonly Pengaju _pemasok;
    private readonly List<Barang> _barangs;
    private readonly Pengajuan _pengajuan;
    
    public QuantityStockTest(MyDbFixture fixture)
    {
        _fixture = fixture;

        using var db = _fixture.CreateContext();
        (_admin, _, _) = db.SeedThreeUser();
        (_pemasok, _) = db.SeedDuaPengaju();
        _barangs = db.SeedBarang20WithQuantity20();

        var cache = new SqliteCache(new SqliteCacheOptions());

        _pengajuan = new Pengajuan(
            db: db,
            pengaju: new Pengaju(nama: "pengaju", isPemasok: true),
            status: StatusPengajuan.Diterima,
            user: _admin,
            barangAjuans: new List<BarangAjuan>
            {
                new BarangAjuan(barang: _barangs.First(), quantity: 1, keterangan: null)
            },
            timeProvider: TimeProvider.System
        );
        db.Add(_pengajuan);
        db.SaveChanges();
    }

    [Fact]
    public void Test_Ketika_Suatu_Pengajuan_Di_Hapus_Maka_Stock_Barangnya_Akan_Kembali()
    {
        using var db = _fixture.CreateContext();
        var cache = new SqliteCache(new SqliteCacheOptions());
        cache.Clear();

        var controller = new DeletePengajuanController(
            db, new MockHttpContextAccessor(_admin)
        );

        var actionResult = controller.Index(db.Pengajuans.Single().Id);
        
        Assert.IsType<OkObjectResult>(actionResult);

        db.ChangeTracker.Clear();
        
        Assert.Empty(db.Pengajuans);
        Assert.Empty(db.BarangAjuans);
        
        // Mastiin instance-instance ini enggak ada yang kehapus
        Assert.Equal(3 , db.Pengajus.Count());
        Assert.Equal(3 , db.Users.Count());
        Assert.Equal(3 , db.StatusPengajuans.Count());
        Assert.Equal(20 , db.Barangs.Count());
        
        // Mastiin barang di kembaliin
        Assert.Single(db.Barangs.Where(barang => barang.CurrentStock == 19));
    }

    public void Dispose()
    {
        _fixture.Cleanup();
    }
}