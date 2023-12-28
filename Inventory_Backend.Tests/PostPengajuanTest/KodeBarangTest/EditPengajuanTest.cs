using Inventory_Backend_NET.Fitur.Pengajuan.PostPengajuan;
using Inventory_Backend.Tests.TestConfiguration.Constant;
using Inventory_Backend.Tests.TestConfiguration.Fixture;
using Inventory_Backend.Tests.TestConfiguration.Mock;
using Microsoft.AspNetCore.Mvc;
using NeoSmart.Caching.Sqlite;

namespace Inventory_Backend.Tests.PostPengajuanTest.KodeBarangTest;

[Collection(TestConstant.WithDbCollection)]
public class EditPengajuanTest : IDisposable
{
    private readonly KodeBarangTestSetupData _testSetupData;
    public EditPengajuanTest(MyDbFixture fixture)
    {
        _testSetupData = new KodeBarangTestSetupData(fixture);
    }
    
    [Fact]
    public void Test_Ketika_Edit_Pengajuan_Maka_Kode_Transaksi_Tidak_Akan_Bertambah()
    {
        using var db = _testSetupData.Fixture.CreateContext();

        var controller = new PostPengajuanController(
            db: db,
            cache: new SqliteCache(new SqliteCacheOptions()),
            httpContextAccessor: new MockHttpContextAccessor(_testSetupData.Admin),
            timeProvider: _testSetupData.MockTimeProvider
        );

        var objectResult = controller.Index(new SubmitPengajuanBody
        {
            IdPengaju = _testSetupData.Pemasok.Id,
            BarangAjuans = new List<BarangAjuanBody>
            {
                new BarangAjuanBody
                {
                    Quantity = 1,
                    IdBarang = _testSetupData.Barangs.First().Id
                }
            }
        });
        Assert.IsType<OkObjectResult>(objectResult);
        
        db.ChangeTracker.Clear();
        var idPengajuan = db.Pengajuans.Single().Id;

        objectResult = controller.Index(new SubmitPengajuanBody
        {
            IdPengajuan = idPengajuan,
            IdPengaju = _testSetupData.Pemasok.Id,
            BarangAjuans = new List<BarangAjuanBody>
            {
                new BarangAjuanBody
                {
                    Quantity = 1,
                    IdBarang = _testSetupData.Barangs.First().Id
                }
            }
        });
        Assert.IsType<OkObjectResult>(objectResult);
        db.ChangeTracker.Clear();

        var pengajuans = db.Pengajuans.ToArray();
        Assert.Single(pengajuans);
        Assert.Equal("TRX-IN-2000-01-01-001" , pengajuans[0].KodeTransaksi);
        Assert.Equal("2000-01-01" , db.TotalPengajuanByTanggals.Single().Tanggal);
        Assert.Equal(1 , db.TotalPengajuanByTanggals.Single().Total);
    }

    public void Dispose()
    {
        _testSetupData.Fixture.Cleanup();
    }
}