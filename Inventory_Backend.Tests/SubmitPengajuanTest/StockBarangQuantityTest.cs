using Inventory_Backend_NET.Controllers.Pengajuan;
using Inventory_Backend_NET.Models;
using Inventory_Backend.Tests.TestConfiguration.Constant;
using Inventory_Backend.Tests.TestConfiguration.Fixture;
using Inventory_Backend.Tests.TestConfiguration.Mock;
using Inventory_Backend.Tests.TestConfiguration.Seeder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Xunit.Abstractions;

namespace Inventory_Backend.Tests.SubmitPengajuanTest;

[Collection(TestConstant.CollectionName)]
public class StockBarangQuantityTest : IDisposable
{
    private User Admin { get; }
    private User NonAdmin { get; }
    private IDistributedCache Cache { get;  }
    private Pengaju Grup { get; }
    private ICollection<Barang> Barangs { get; }
    public TransactionalMyDbFixture Fixture { get; }
    
    private readonly ITestOutputHelper _testOutputHelper;

    public StockBarangQuantityTest(
        TransactionalMyDbFixture fixture, ITestOutputHelper testOutputHelper)
    {
        Fixture = fixture;
        _testOutputHelper = testOutputHelper;

        using var db = fixture.CreateContext();
        (Admin , NonAdmin , _) = db.SeedThreeUser();
        db.SeedBarang20WithQuantity20();
        db.SeedDuaPengaju();

        Grup = db.Pengajus.Single(pengaju => !pengaju.IsPemasok);
        Barangs = db.Barangs.ToList();
        Cache = new MockDistributedCache();
    }
    
    
    [Fact]
    public void Test_Ketika_Submit_Pengajuan_Baru_Dengan_Pengaju_Tipe_Grup_Berhasil_Maka_Current_Stock_Akan_Berkurang()
    {
        var barang1 = Barangs.First();
        var barang2 = Barangs.Last();
        var pengajuan = new SubmitPengajuanBody
        {
            IdPengaju = Grup.Id,
            BarangAjuans = [
                new BarangAjuanBody
                {
                    IdBarang = barang1.Id,
                    Quantity = 2
                },
                new BarangAjuanBody
                {
                    IdBarang = barang2.Id,
                    Quantity = 4
                }
            ]
        };

        var userContext = new MockHttpContextAccessor(NonAdmin);
        using var db = Fixture.CreateContext();
        
        var controller = new PostPengajuanController(
            db: db,
            cache: Cache,
            httpContextAccessor: userContext
        );
        var result = controller.Index(requestBody: pengajuan);
        Assert.IsType<OkObjectResult>(result);
        
        Assert.NotNull(
            db.Barangs.Single(barang => 
                barang.CurrentStock == 18 &&
                barang.Id == barang1.Id
            )
        );
        Assert.NotNull(
            db.Barangs.Single(barang => 
                barang.CurrentStock == 16 &&
                barang.Id == barang2.Id
            )
        );
    }

    public void Dispose()
    {
        Fixture.Cleanup();
    }
}