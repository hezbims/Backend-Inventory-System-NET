using Inventory_Backend_NET.Controllers.Pengajuan;
using Inventory_Backend_NET.Models;
using Inventory_Backend.Tests.TestConfiguration.Constant;
using Inventory_Backend.Tests.TestConfiguration.Fixture;
using Inventory_Backend.Tests.TestConfiguration.Mock;
using Inventory_Backend.Tests.TestConfiguration.Seeder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using NeoSmart.Caching.Sqlite;

namespace Inventory_Backend.Tests.PostPengajuanTest.StatusPengajuanTest;

[Collection(TestConstant.WithDbCollection)]
public class TestStatusBuatPengajuanBaru : IDisposable
{
    private readonly MyDbFixture _fixture;
    private readonly User _admin;
    private readonly User _nonAdmin;
    private readonly List<Barang> _barangs;
    private readonly Pengaju _grup;
    private readonly IDistributedCache _cache;
    
    public TestStatusBuatPengajuanBaru(MyDbFixture fixture)
    {
        _fixture = fixture;
        _cache = new SqliteCache(new SqliteCacheOptions());

        using var db = _fixture.CreateContext();
        (_admin , _nonAdmin , _) = db.SeedThreeUser();
        (_grup, _) = db.SeedDuaPengaju();

        _barangs = db.SeedBarang20WithQuantity20();
    }

    [Fact]
    public void Test_Kalau_Admin_Yang_Buat_Pengajuan_Baru_Maka_Statusnya_Diterima()
    {
        using var db = _fixture.CreateContext();

        var adminContext = new MockHttpContextAccessor(_admin);
        var controller = new PostPengajuanController(
            db: db, 
            httpContextAccessor: adminContext, 
            cache: _cache,
            timeProvider: TimeProvider.System
        );

        var actionResult = controller.Index(new SubmitPengajuanBody
        {
            BarangAjuans = new List<BarangAjuanBody>()
            {
                new BarangAjuanBody
                {
                    IdBarang = _barangs.First().Id,
                    Keterangan = null,
                    Quantity = 1
                }
            },
            IdPengaju = _grup.Id
        });

        Assert.IsType<OkObjectResult>(actionResult);

        var pengajuan = db.Pengajuans.Single();
        Assert.Equal(StatusPengajuan.DiterimaValue , pengajuan.Status.Value);
    }

    [Fact]
    public void Test_Kalau_Non_Admin_Buat_Pengajuan_Baru_Maka_Statusnya_Menunggu()
    {
        using var db = _fixture.CreateContext();

        var nonAdminContext = new MockHttpContextAccessor(_nonAdmin);
        var controller = new PostPengajuanController(
            db: db, 
            httpContextAccessor: nonAdminContext, 
            cache: _cache,
            timeProvider: TimeProvider.System
        );
        
        var actionResult = controller.Index(new SubmitPengajuanBody
        {
            BarangAjuans = new List<BarangAjuanBody>
            {
                new BarangAjuanBody
                {
                    IdBarang = _barangs.First().Id,
                    Keterangan = null,
                    Quantity = 1
                }
            },
            IdPengaju = _grup.Id
        });

        Assert.IsType<OkObjectResult>(actionResult);
        var pengajuan = db.Pengajuans.Single();
        Assert.Equal(StatusPengajuan.MenungguValue , pengajuan.Status.Value);
    }

    public void Dispose()
    {
        _fixture.Cleanup();
    }
}