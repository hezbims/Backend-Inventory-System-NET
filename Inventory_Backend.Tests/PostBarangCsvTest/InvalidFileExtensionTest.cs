using Inventory_Backend_NET.Controllers.Barang;
using Inventory_Backend.Tests.TestConfiguration.Constant;
using Inventory_Backend.Tests.TestConfiguration.Fixture;
using Inventory_Backend.Tests.TestAssets;
using Microsoft.AspNetCore.Mvc;
using Xunit.Abstractions;

namespace Inventory_Backend.Tests.PostBarangCsvTest;

[Collection(TestConstant.CollectionName)]
public class InvalidFileExtensionTest : IDisposable
{
    private readonly TransactionalMyDbFixture _fixture;
    private readonly ITestOutputHelper _logger;
    public InvalidFileExtensionTest(
        TransactionalMyDbFixture fixture,
        ITestOutputHelper logger
    )
    {
        _fixture = fixture;
        _logger = logger;
    }

    [Fact]
    public void Test_Invalid_File_Extension()
    {
        // var db = _fixture.CreateContext();
        // var postCsvController = new PostBarangByCsvController(db);
        //
        // var formFile = TestAssetsUtils.GetFormFile(
        //     filename: "incorrect_file_extension.txt",
        //     jsonKey: "csv"
        // );
        // var result = postCsvController.Index(new PostBarangByCsvController.CsvUploadModel { Csv = null });
        //
        // var badRequestValue = result;//.Value as PostBarangByCsvController.ErrorModel;

        // Assert.Equal(
        //     expected: "Extension file yang diperbolehkan hanya : .csv", 
        //     actual: badRequestValue!.Errors.First()
        // );


    }

    public void Dispose()
    {
        _fixture.Cleanup();
    }
}