using Inventory_Backend_NET.Controllers.Pengajuan;
using Inventory_Backend_NET.Models;
using Inventory_Backend.Tests.TestConfiguration.Constant;
using Inventory_Backend.Tests.TestConfiguration.Fixture;
using Inventory_Backend.Tests.TestConfiguration.Mock;
using Inventory_Backend.Tests.TestConfiguration.Seeder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using NeoSmart.Caching.Sqlite;

namespace Inventory_Backend.Tests.PostPengajuanTest;

[Collection(TestConstant.WithDbCollection)]
public class EditPengajuanStockQuantityTest : IDisposable
{
    private readonly MyDbFixture _fixture;
    private readonly IDistributedCache _cache;
    private readonly List<Barang> _barangs;
    private readonly User _admin;
    
    public EditPengajuanStockQuantityTest(MyDbFixture fixture)
    {
        _fixture = fixture;
        _cache = new SqliteCache(new SqliteCacheOptions());

        using var db = _fixture.CreateContext();

        (_admin, _ , _) = db.SeedThreeUser();
        var (_ , grup) = db.SeedDuaPengaju();
        _barangs = db.SeedBarang20WithQuantity20();

        db.Pengajuans.Add(new Pengajuan(
            cache: _cache,
            pengaju: grup,
            status: StatusPengajuan.Diterima, 
            user: _admin,
            barangAjuans: new List<BarangAjuan>
            {
                new BarangAjuan(barang: _barangs[0] , quantity: 2, keterangan: "hai"),
                new BarangAjuan(barang: _barangs[1], quantity: 3, keterangan: null)
            }
        ));
        db.SaveChanges();
    }

    [Fact]
    public void Test_Edit_Pengajuan_Stock_Terupdate_Dengan_Benar()
    {
        using var db = _fixture.CreateContext();

        var adminContext = new MockHttpContextAccessor(_admin);
        var controller = new PostPengajuanController(db, adminContext, _cache);

        var pemasok = db.Pengajus.Single(pengaju => pengaju.IsPemasok);
        var pengajuan = db.Pengajuans.Single();

        var actionResult = controller.Index(new SubmitPengajuanBody
        {
            IdPengaju = pemasok.Id,
            IdPengajuan = pengajuan.Id,
            BarangAjuans = new List<BarangAjuanBody>
            {
                new BarangAjuanBody { IdBarang = _barangs[0].Id, Keterangan = "hai", Quantity = 3 },
                new BarangAjuanBody { IdBarang = _barangs[2].Id, Keterangan = null, Quantity = 5 }
            }
        });

        Assert.IsType<OkObjectResult>(actionResult);
        Assert.Equal(2 , db.BarangAjuans.Count());
        Assert.Single(db.Pengajuans);

        Assert.Single(db.Barangs.Where(barang =>
            barang.Id == _barangs[0].Id &&
            barang.CurrentStock == 25
        ));
        Assert.Single(db.Barangs.Where(barang => 
            barang.Id == _barangs[1].Id &&
            barang.CurrentStock == 23
        ));
        Assert.Single(db.Barangs.Where(barang => 
            barang.Id == _barangs[2].Id &&
            barang.CurrentStock == 25
        ));
        Assert.Equal(17 , db.Barangs.Where(barang => 
            barang.CurrentStock == 20
        ).Count());
    }
    
    public void Dispose()
    {
        _fixture.Cleanup();
    }   
}