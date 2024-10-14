using Inventory_Backend_NET.Fitur.Barang.PostBarangByCsv;
using Inventory_Backend.Tests.TestConfiguration;
using Inventory_Backend.Tests.TestConfiguration.Constant;
using Inventory_Backend.Tests.TestConfiguration.Fixture;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Backend.Tests.PostBarangCsvTest;

[Collection(TestConstant.UnitTestWithDbCollection)]
public class BerhasilOverwriteTest : IDisposable
{
    private readonly MyDbFixture _fixture;
    public BerhasilOverwriteTest(
        MyDbFixture fixture    
    )
    {
        _fixture = fixture;
    }

    [Fact]
    public void Test_Berhasil_Overwrite_Berdasarkan_Kode_Barang()
    {
        var db = _fixture.CreateContext();

        var formFile = TestAssetsUtils.GetDuaBarangSamaCsv();
        var controller = new PostBarangByCsvController(db);

        var actionResult = controller.Index(new PostBarangByCsvController.CsvUploadModel
        {
            Csv = formFile,
            OverWriteByKodeBarang = true
        });

        Assert.IsType<OkObjectResult>(actionResult);

        var allBarang = db.Barangs.Include(barang => barang.Kategori).ToList();
        Assert.Single(allBarang);
        Assert.Single(allBarang.Where(
            barang => 
                barang.KodeBarang == "R1-1-1" &&
                barang.Nama == "Oli" &&
                barang.Kategori.Nama == "Oli" &&
                barang.NomorRak == 1 &&
                barang.NomorLaci == 1 &&
                barang.NomorKolom == 1 &&
                barang.CurrentStock == 5 &&
                barang.MinStock == 1 &&
                barang.LastMonthStock == 5 &&
                barang.UnitPrice == 5000 &&
                barang.Uom == "Liter"
        ));
    }

    public void Dispose()
    {
        _fixture.Cleanup();
    }
}