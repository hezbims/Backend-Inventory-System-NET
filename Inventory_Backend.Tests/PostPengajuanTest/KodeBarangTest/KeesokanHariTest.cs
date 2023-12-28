using Inventory_Backend_NET.Database.Models;
using Inventory_Backend_NET.Fitur.Pengajuan.PostPengajuan;
using Inventory_Backend.Tests.TestConfiguration.Constant;
using Inventory_Backend.Tests.TestConfiguration.Fixture;
using Inventory_Backend.Tests.TestConfiguration.Mock;
using Inventory_Backend.Tests.TestConfiguration.Seeder;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NeoSmart.Caching.Sqlite;

namespace Inventory_Backend.Tests.PostPengajuanTest.KodeBarangTest;

[Collection(TestConstant.WithDbCollection)]
public class KeesokanHariTest : IDisposable
{
    private readonly MyDbFixture _fixture;
    private readonly SqliteCache _cache;
    private readonly User _nonAdmin;
    private readonly Pengaju _grup;
    private readonly Barang[] _barangs;



    public KeesokanHariTest(MyDbFixture fixture)
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

        _barangs = db.Barangs.ToArray();
    }

    [Fact]
    public void Test_Ketika_Mulai_Mengajukan_DiKeesokan_Hari_Maka_Mulai_Dari_1_Lagi()
    {
         var today = new DateTimeOffset(
            year: 2000, month: 1, day: 1, hour: 0, minute: 0, second: 0,
            offset: TimeSpan.Zero
        );
        var tomorrow = today.AddDays(1);
        var mockTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Jakarta");

        

        var isFirstDay = true;
        var mockTimeProvider = new Mock<TimeProvider>();
        mockTimeProvider.Setup(provider => provider.GetUtcNow())
            .Returns(() => isFirstDay ? today : tomorrow);
        mockTimeProvider.Setup(provider => provider.LocalTimeZone).Returns(mockTimeZone);

        using var db = _fixture.CreateContext();
        var controller = new PostPengajuanController(
            db : db,
            httpContextAccessor: new MockHttpContextAccessor(_nonAdmin),
            cache: _cache,
            timeProvider: mockTimeProvider.Object
        );

        var todayActionResult = controller.Index(new SubmitPengajuanBody
        {
            IdPengaju = _grup.Id,
            BarangAjuans = new List<BarangAjuanBody>
            {
                new BarangAjuanBody
                {
                    Quantity = 1, IdBarang = _barangs.First().Id
                }
            }
        });
        isFirstDay = false;
        var tomorrowActionResult = controller.Index(new SubmitPengajuanBody
        {
            IdPengaju = _grup.Id,
            BarangAjuans = new List<BarangAjuanBody>
            {
                new BarangAjuanBody
                {
                    Quantity = 1, IdBarang = _barangs.First().Id
                }
            }
        });
        Assert.IsType<OkObjectResult>(todayActionResult);
        Assert.IsType<OkObjectResult>(tomorrowActionResult);
        
        Assert.Single(db.Pengajuans.Where(pengajuan => 
            pengajuan.KodeTransaksi == "TRX-OUT-2000-01-01-001"
        ));
        Assert.Single(db.Pengajuans.Where(pengajuan => 
            pengajuan.KodeTransaksi == "TRX-OUT-2000-01-02-001"
        ));
    }

    public void Dispose()
    {
        _fixture.Cleanup();
    }
}