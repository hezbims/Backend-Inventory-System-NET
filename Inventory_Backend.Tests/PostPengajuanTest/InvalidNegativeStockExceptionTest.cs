using Inventory_Backend_NET.Database.Models;
using Inventory_Backend_NET.Fitur.Pengajuan.PostPengajuan;
using Inventory_Backend.Tests.TestConfiguration.Constant;
using Inventory_Backend.Tests.TestConfiguration.Fixture;
using Inventory_Backend.Tests.TestConfiguration.Mock;
using Inventory_Backend.Tests.TestConfiguration.Seeder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using NeoSmart.Caching.Sqlite;

namespace Inventory_Backend.Tests.PostPengajuanTest;

[Collection(TestConstant.WithDbCollection)]
public class InvalidNegativeStockExceptionTest : IDisposable
{
    private readonly MyDbFixture _fixture;
    private readonly IHttpContextAccessor _adminContext;
    private readonly IDistributedCache _cache;
    private readonly Pengaju _grup;
    private readonly Barang _barang;
    
    public InvalidNegativeStockExceptionTest(MyDbFixture fixture)
    {
        _fixture = fixture;

        using var db = _fixture.CreateContext();
        
        db.SeedBarang20WithQuantity20();
        _barang = db.Barangs.First();
        
        var (admin , _ , _) = db.SeedThreeUser();
        _adminContext = new MockHttpContextAccessor(admin);

        db.SeedDuaPengaju();
        _grup = db.Pengajus.First(pengaju => !pengaju.IsPemasok);

        _cache = new SqliteCache(new SqliteCacheOptions());
    }

    [Fact]
    public void Test_Tambah_Ambil_Barang_Dengan_Quantity_21()
    {
        using var db = _fixture.CreateContext();
        var controller = new PostPengajuanController(
            db: db,
            cache: _cache,
            httpContextAccessor: _adminContext,
            timeProvider: TimeProvider.System
        );


        var actionResult = controller.Index(new SubmitPengajuanBody
        {
            IdPengaju = _grup.Id,
            BarangAjuans = new []
            {
                new BarangAjuanBody
                {
                    Quantity = 21,
                    Keterangan = null,
                    IdBarang = _barang.Id 
                }
            }
        });
        
        Assert.IsType<BadRequestObjectResult>(actionResult);

        db.ChangeTracker.Clear();
        Assert.Empty(db.Pengajuans.ToArray());
        Assert.All(
            collection: db.Barangs.ToArray() , 
            action: barang => Assert.Equal(20 , barang.CurrentStock)
        );
    }

    public void Dispose()
    {
        _fixture.Cleanup();
    }
}