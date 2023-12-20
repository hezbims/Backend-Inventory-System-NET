using Inventory_Backend_NET.Controllers.Pengajuan;
using Inventory_Backend_NET.Models;
using Inventory_Backend.Tests.TestConfiguration.Constant;
using Inventory_Backend.Tests.TestConfiguration.Fixture;
using Inventory_Backend.Tests.TestConfiguration.Mock;
using Inventory_Backend.Tests.TestConfiguration.Seeder;
using Microsoft.Extensions.Caching.Distributed;
using NeoSmart.Caching.Sqlite;

namespace Inventory_Backend.Tests.PostPengajuanTest.KodeBarangTest;

[Collection(TestConstant.WithDbCollection)]
public class KodeBarangTest : IDisposable
{
    private readonly MyDbFixture _fixture;
    private readonly SqliteCache _cache;
    private readonly User _nonAdmin;
    private readonly Pengaju _grup;
    private readonly Pengaju _pemasok;
    private readonly Barang[] _barangs;

    public KodeBarangTest(MyDbFixture fixture)
    {
        _fixture = fixture;
        _cache = new SqliteCache(new SqliteCacheOptions());
        _cache.Clear();

        using var db = _fixture.CreateContext();
        db.SeedThreeUser();
        db.SeedDuaPengaju();
        db.SeedBarang20WithQuantity20();
        _nonAdmin = db.Users.First(user => !user.IsAdmin);
        _grup = db.Pengajus.First(pengaju => !pengaju.IsPemasok);
        _pemasok = db.Pengajus.First(pengaju => pengaju.IsPemasok);

        _barangs = db.Barangs.ToArray();
    }

    [Fact]
    public void Test_Kode_Urutan_Pengajuan()
    {
        using var db = _fixture.CreateContext();
        
        var controller = new PostPengajuanController(
            db: db,
            cache: _cache,
            httpContextAccessor: new MockHttpContextAccessor(_nonAdmin) 
        );

        controller.Index(new SubmitPengajuanBody
        {
            IdPengajuan = null,
            IdPengaju = _grup.Id,
            BarangAjuans = new List<BarangAjuanBody>
            {
                new BarangAjuanBody
                {
                    IdBarang = _barangs[0].Id,
                    Keterangan = null,
                    Quantity = 1
                }
            }
        });
        TimeProvider.System.LocalTimeZone.To

        Assert.Single(pengajuan => pengajuan.);
    }

    public void Dispose()
    {
        _fixture.Cleanup();
    }
}