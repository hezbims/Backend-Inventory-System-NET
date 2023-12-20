using Inventory_Backend_NET.Controllers.Pengajuan;
using Inventory_Backend_NET.Models;
using Inventory_Backend.Tests.TestConfiguration.Constant;
using Inventory_Backend.Tests.TestConfiguration.Fixture;
using Inventory_Backend.Tests.TestConfiguration.Mock;
using Inventory_Backend.Tests.TestConfiguration.Seeder;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NeoSmart.Caching.Sqlite;

namespace Inventory_Backend.Tests.PostPengajuanTest.KodeBarangTest;

[Collection(TestConstant.WithDbCollection)]
public class SebelumnyaGagalTest : IDisposable
{
    private readonly MyDbFixture _fixture;
    private readonly SqliteCache _cache;
    private readonly User _admin;
    private readonly Pengaju _pemasok;
    private readonly Barang[] _barangs;

    private readonly DateTimeOffset _mockDateTime = new DateTimeOffset(
        year: 2000, month: 1, day: 1, hour: 0, minute: 0, second: 0,
        offset: TimeSpan.Zero
    );
    private readonly TimeZoneInfo _mockTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Jakarta");
    
    public SebelumnyaGagalTest(MyDbFixture fixture)
    {
        _fixture = fixture;
        _cache = new SqliteCache(new SqliteCacheOptions());
        _cache.Clear();

        using var db = _fixture.CreateContext();
        db.SeedThreeUser();
        db.SeedDuaPengaju();
        db.SeedBarang20WithQuantity20();
        _admin = db.Users.First(user => user.IsAdmin);
        _pemasok = db.Pengajus.First(pengaju => pengaju.IsPemasok);

        _barangs = db.Barangs.ToArray();
    }

    [Fact]
    public void Test_Ketika_Pengajuan_Sebelumnya_Gagal_Maka_Urutan_Sebelumnya_Enggak_Kehitung_Dan_Tetep_Mulai_Dari_Satu_Lagi()
    {
        using var db = _fixture.CreateContext();

        var mockTimeProvider = new Mock<TimeProvider>();
        mockTimeProvider.Setup(provider => provider.GetUtcNow()).Returns(_mockDateTime);
        mockTimeProvider.Setup(provider => provider.LocalTimeZone).Returns(_mockTimeZone);
        
        var controller = new PostPengajuanController(
            db: db,
            cache: _cache,
            httpContextAccessor: new MockHttpContextAccessor(_admin),
            timeProvider: mockTimeProvider.Object
        );

        var failedActionResult = controller.Index(new SubmitPengajuanBody());
        Assert.IsType<BadRequestObjectResult>(failedActionResult);

        var okActionResult = controller.Index(new SubmitPengajuanBody
        {
            IdPengaju = _pemasok.Id,
            BarangAjuans = new List<BarangAjuanBody>
            {
                new BarangAjuanBody
                {
                    Quantity = 1,
                    IdBarang = _barangs.First().Id
                }
            }
        });
        Assert.IsType<OkObjectResult>(okActionResult);

        var pengajuan = db.Pengajuans.Single();
        Assert.Equal("TRX-IN-2000-01-01-001" , pengajuan.KodeTransaksi);
    }

    public void Dispose()
    {
        _fixture.Cleanup();
    }
}