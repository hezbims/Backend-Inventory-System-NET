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
public class MengajukanTigaKaliTest : IDisposable
{

    private readonly MyDbFixture _fixture;
    private readonly User _admin;
    private readonly Pengaju _pemasok;
    private readonly List<Barang> _barangs;
    
    private readonly DateTimeOffset _mockDateTime = new DateTimeOffset(
        year: 2000, month: 1, day: 1, hour: 0, minute: 0, second: 0,
        offset: TimeSpan.Zero
    );
    private readonly TimeZoneInfo _mockTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Jakarta");
    
    public MengajukanTigaKaliTest(
        MyDbFixture fixture    
    )
    {
        _fixture = fixture;
        using var db = _fixture.CreateContext();

        (_admin , _ , _) = db.SeedThreeUser();
        (_pemasok , _) = db.SeedDuaPengaju();
        _barangs = db.SeedBarang20WithQuantity20();
    }

    [Fact]
    public void Test_Ketika_Mengajukan_Sukses_Tiga_Kali_Maka_Setiap_Transaksi_Kode_Transaksinya_Benar()
    {
        using var db = _fixture.CreateContext();

        var mockTimeProvider = new Mock<TimeProvider>();
        mockTimeProvider.Setup(p => p.GetUtcNow()).Returns(_mockDateTime);
        mockTimeProvider.Setup(p => p.LocalTimeZone).Returns(_mockTimeZone);

        var controller = new PostPengajuanController(
            db: db,
            httpContextAccessor: new MockHttpContextAccessor(_admin),
            cache: new SqliteCache(new SqliteCacheOptions()),
            timeProvider: mockTimeProvider.Object
        );


        var results = Enumerable.Range(1, 3).Select(_ =>
            controller.Index(new SubmitPengajuanBody
            {
                IdPengaju = _pemasok.Id,
                BarangAjuans = new List<BarangAjuanBody>
                {
                    new BarangAjuanBody
                    {
                        IdBarang = _barangs.First().Id,
                        Quantity = 1
                    }
                }
            })    
        );
        Assert.All(results , result => Assert.IsType<OkObjectResult>(result));

        var expectedKodeTransaksi = new[]
        {
            "TRX-IN-2000-01-01-001",
            "TRX-IN-2000-01-01-002",
            "TRX-IN-2000-01-01-003"
        };

        var pengajuans = db.Pengajuans.ToArray();
        for (var i = 0 ; i < 3 ; i++)
            Assert.Equal(expectedKodeTransaksi[i] , pengajuans[i].KodeTransaksi);

    }

    public void Dispose()
    {
        _fixture.Cleanup();
    }
}