using Inventory_Backend_NET.Controllers.Barang;
using Inventory_Backend_NET.Controllers.Barang.PostBarangByCsv;
using Inventory_Backend.Tests.TestConfiguration.Constant;
using Inventory_Backend.Tests.TestConfiguration.Fixture;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit.Abstractions;

namespace Inventory_Backend.Tests.PostBarangCsvTest;

[Collection(TestConstant.CollectionName)]
public class PostTemplateCsvTest : IDisposable
{
    public readonly TransactionalMyDbFixture Fixture;
    public readonly ITestOutputHelper Logger;
    public PostTemplateCsvTest(
        TransactionalMyDbFixture fixture,
        ITestOutputHelper logger
    )
    {
        Fixture = fixture;
        Logger = logger;
    }

    [Fact]
    public void Test_Submit_Template_Csv_Masuk_Ke_Database()
    {
        var testProjectPath = Directory.GetParent(Environment.CurrentDirectory)!
            .Parent!
            .Parent!
            .Parent!
            .FullName;

        var mainProjectPath = Path.Combine(
            testProjectPath,
            "Inv_Backend_NET/"
        );

        var mockEnvironment = new Mock<IWebHostEnvironment>();
        mockEnvironment
            .Setup(env => env.ContentRootPath)
            .Returns(mainProjectPath);

        var getCsvTemplateController = new GetTemplateBarangCsvController(mockEnvironment.Object);

        var csvFileResult = getCsvTemplateController.Index();
        var fileStream = new MemoryStream(csvFileResult.FileContents);
        
        var formFile = new FormFile(
            baseStream: fileStream,
            baseStreamOffset: 0,
            length: fileStream.Length,
            name: "csv",
            fileName: "template_input_barang.csv"
        );

        var db = Fixture.CreateContext();
        var postBarangByCsvController = new PostBarangByCsvController(
            db: db
        );

        var uploadResult = postBarangByCsvController.Index(new PostBarangByCsvController.CsvUploadModel()
        {
            Csv = formFile
        });

        Assert.IsType<OkObjectResult>(uploadResult);
        
        var barangInDb = db.Barangs.Include(barang => barang.Kategori).ToList();
        
        Assert.Single(barangInDb.Where(
            barang =>
                barang.KodeBarang == "R1-1-1" &&
                barang.Nama == "Mata Solder" &&
                barang.Kategori.Nama == "Solder" &&
                barang.NomorRak == 1 &&
                barang.NomorLaci == 1 &&
                barang.NomorKolom == 1 &&
                barang.CurrentStock == 45 &&
                barang.MinStock == 10 &&
                barang.LastMonthStock == 55 &&
                barang.UnitPrice == 10000 &&
                barang.Uom == "Pc"
        ));
    } 

    public void Dispose()
    {   
        Fixture.Cleanup();
    }
}