using Inventory_Backend_NET.Fitur.Barang.PostBarangByCsv;
using Inventory_Backend.Tests.TestConfiguration;
using Inventory_Backend.Tests.TestConfiguration.Constant;
using Inventory_Backend.Tests.TestConfiguration.Fixture;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Backend.Tests.PostBarangCsvTest;

[Collection(TestConstant.UnitTestWithDbCollection)]
public class InvalidFieldTest : IDisposable
{
    private readonly MyDbFixture _fixture;
    public InvalidFieldTest(MyDbFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Test_Invalid_Csv_Field()
    {
        var formFile = TestAssetsUtils.GetFormFile(filename: "invalid_field.csv", jsonKey: "csv");

        var db = _fixture.CreateContext();
        var controller = new PostBarangByCsvController(db);

        var actionResult = (controller.Index(new PostBarangByCsvController.CsvUploadModel
        {
            Csv = formFile,
            OverWriteByKodeBarang = true
        }) as BadRequestObjectResult)!;

        var errorModel = (actionResult.Value as PostBarangByCsvController.ErrorModel)!;
        var baris3 = errorModel.Errors["BARIS #3"];
        Assert.Contains("Kode Barang tidak boleh kosong", baris3);
        Assert.Contains("Nama Barang tidak boleh kosong", baris3);
        Assert.Contains("Kategori tidak boleh kosong", baris3);
        Assert.Contains("Nomor Rak harus di rentang 1-6", baris3);
        Assert.Contains("Nomor Laci harus di rentang 1-30" , baris3);
        Assert.Contains("Nomor Kolom harus di rentang 1-9", baris3);
        Assert.Equal(6 , baris3.Count);

        var baris4 = errorModel.Errors["BARIS #4"];
        Assert.Contains("Nomor Kolom harus di rentang 1-9", baris4);
        Assert.Contains("Current Stock tidak valid", baris4);
        Assert.Contains("Min. Stock tidak valid", baris4);
        Assert.Contains("Last Month Stock tidak valid", baris4);
        Assert.Contains("Unit Price tidak valid", baris4);
        Assert.Contains("UOM tidak boleh kosong", baris4);
        Assert.Equal(6 , baris4.Count);
        
        Assert.Empty(db.Barangs.ToList());
    }

    public void Dispose()
    {
        _fixture.Cleanup();
    }
}