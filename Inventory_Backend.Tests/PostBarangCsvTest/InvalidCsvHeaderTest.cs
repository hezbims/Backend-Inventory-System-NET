using Inventory_Backend_NET.Controllers.Barang;
using Inventory_Backend_NET.Models;
using Inventory_Backend.Tests.TestConfiguration.Constant;
using Inventory_Backend.Tests.TestConfiguration.Fixture;
using Inventory_Backend.Tests.TestConfiguration.Mock;
using Inventory_Backend.Tests.TestConfiguration.Seeder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit.Abstractions;

namespace Inventory_Backend.Tests.PostBarangCsvTest;

[Collection(TestConstant.WithDbCollection)]
public class InvalidCsvHeaderTest : IDisposable
{
    private readonly MyDbFixture _fixture;
    private readonly ITestOutputHelper _logger;
    public User Admin;
    
    public InvalidCsvHeaderTest(
        MyDbFixture fixture,
        ITestOutputHelper logger
    )
    {
        _fixture = fixture;
        _logger = logger;
        var db = _fixture.CreateContext();
        (Admin, _, _) = db.SeedThreeUser();
    }

    [Fact]
    public void Test_Invalid_Header()
    {
        var mockHttpContextAccessor = new MockHttpContextAccessor(Admin);
        
        var db = _fixture.CreateContext();
        var postCsvController = new PostBarangByCsvController(db);

        var csvFile = File.OpenRead("./TestAssets/without_header.csv");
        var result = (ObjectResult)postCsvController.Index(new PostBarangByCsvController.CsvUploadModel
        {
            Csv = new FormFile(
                baseStream: csvFile,
                baseStreamOffset: 0,
                length: csvFile.Length,
                name: "csv",
                fileName: csvFile.Name
            )
        });
        _logger.WriteLine((result.Value as PostBarangByCsvController.ErrorModel)!.Errors["HEADER"].First());

        var value = result.Value! as PostBarangByCsvController.ErrorModel;
        
        Assert.Equal(
            "Header 'KODE BARANG, NAMA BARANG, KATEGORI, NOMOR RAK, NOMOR LACI, " +
            "NOMOR KOLOM, CURRENT STOCK, MIN. STOCK, LAST MONTH STOCK, UNIT PRICE, UOM" +
            "' tidak ditemukan dalam CSV",
            value!.Errors["HEADER"].First()
        );
    }

    public void Dispose()
    {
        _fixture.Cleanup();
    }
}