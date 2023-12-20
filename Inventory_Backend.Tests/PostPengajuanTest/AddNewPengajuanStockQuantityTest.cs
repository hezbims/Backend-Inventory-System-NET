using Inventory_Backend_NET.Controllers.Pengajuan;
using Inventory_Backend_NET.Models;
using Inventory_Backend.Tests.TestConfiguration.Constant;
using Inventory_Backend.Tests.TestConfiguration.Fixture;
using Inventory_Backend.Tests.TestConfiguration.Mock;
using Inventory_Backend.Tests.TestConfiguration.Seeder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace Inventory_Backend.Tests.PostPengajuanTest;

/// <summary>
/// Mengecek apakah current stock pada tabel barangs terupdate dengan benar setelah nambah pengajuan baru
/// </summary>
[Collection(TestConstant.WithDbCollection)]
public class AddNewPengajuanStockQuantityTest : IDisposable
{
    private User Admin { get; }
    private User NonAdmin { get; }
    private IDistributedCache Cache { get;  }
    private Pengaju Grup { get; }
    private ICollection<Barang> Barangs { get; }
    public MyDbFixture Fixture { get; }
    
    public AddNewPengajuanStockQuantityTest(
        MyDbFixture fixture
    )
    {
        Fixture = fixture;

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

        var nonAdminContext = new MockHttpContextAccessor(NonAdmin);
        using var db = Fixture.CreateContext();
        
        var controller = new PostPengajuanController(
            db: db,
            cache: Cache,
            httpContextAccessor: nonAdminContext
        );
        var result = controller.Index(requestBody: pengajuan);
        Assert.IsType<OkObjectResult>(result);
        
        Assert.Equal(
            18,
            db.Barangs.Single(barang => barang.Id == barang1.Id).CurrentStock
        );
        Assert.Equal(
            16,
            db.Barangs.Single(barang => barang.Id == barang2.Id).CurrentStock
        );
    }

    public void Dispose()
    {
        Fixture.Cleanup();
    }
}