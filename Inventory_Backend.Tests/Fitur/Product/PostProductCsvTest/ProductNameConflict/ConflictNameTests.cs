using System.Net;
using Inventory_Backend_NET.Database.Models;
using Inventory_Backend.Tests.TestConfiguration;
using Inventory_Backend.Tests.TestConfiguration.Constant;
using Inventory_Backend.Tests.TestConfiguration.Fixture;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace Inventory_Backend.Tests.Fitur.Product.PostProductCsvTest.ProductNameConflict;

/// <summary>
/// Menguji ketika ada nama product yang sama
/// </summary>
[Collection(TestConstant.IntegrationTestDefinition)]
public class ConflictNameTests : IDisposable
{
    private readonly TestWebAppFactory _webAppFactory;
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _output;
    
    public ConflictNameTests(
        TestWebAppFactory factory,
        ITestOutputHelper output)
    {
        _webAppFactory = factory;
        _webAppFactory.ConfigureLoggingToTestOutput(output);
        _output = output;
        _client = factory.CreateClient();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task test_product_name_can_not_be_same_when_product_code_is_different(
        bool isOverwriteBySameProductCode)
    {
        await using var db = _webAppFactory.GetDbContext();
        using var requestBody = new MultipartFormDataContent();
        using var csvStream = TestAssetsUtils.GetFileStream(
            "./Fitur/Product/PostProductCsvTest/ProductNameConflict/_Preparation/different_code_same_name.csv");
        
        requestBody.Add(csvStream, "csv", "file.csv");
        requestBody.Add(new StringContent(isOverwriteBySameProductCode ? "true" : "false"), "overwrite_by_kode_barang");

        var response = await _client.PostAsync(
            TestConstant.ApiEndpoints.Product.PostCsv, requestBody);
        _output.WriteLine(await response.Content.ReadAsStringAsync());
        
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Empty(db.Barangs.ToList());
        Assert.Empty(db.Kategoris.ToList());
    }

    [Fact]
    public async Task test_product_name_can_be_same_when_product_code_is_same()
    {
        await using var db = _webAppFactory.GetDbContext();
        using var requestBody = new MultipartFormDataContent();
        using var csvStream = TestAssetsUtils.GetFileStream(
            "./Fitur/Product/PostProductCsvTest/ProductNameConflict/_Preparation/same_code_same_name.csv");
        
        requestBody.Add(csvStream, "csv", "file.csv");
        requestBody.Add(new StringContent("true"), "overwrite_by_kode_barang");

        var response = await _client.PostAsync(
            TestConstant.ApiEndpoints.Product.PostCsv, requestBody);
        
        _output.WriteLine(await response.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        Barang product = db.Barangs.Include(product1 => product1.Kategori).Single();
        Assert.Equal(2, db.Kategoris.Count());
        Assert.True(product.Id > 0);
        Assert.Equal("R1-1-1", product.KodeBarang);
        Assert.Equal("Mata Solder", product.Nama);
        Assert.Equal("Alat Kecil", product.Kategori.Nama);
        Assert.Equal(1, product.NomorRak);
        Assert.Equal(1, product.NomorLaci);
        Assert.Equal(1, product.NomorKolom);
        Assert.Equal(45, product.CurrentStock);
        Assert.Equal(10, product.MinStock);
        Assert.Equal(55, product.LastMonthStock);
        Assert.Equal(20000, product.UnitPrice);
        Assert.Equal("Pc", product.Uom);
    }

    public void Dispose()
    {
        _client.Dispose();
        _webAppFactory.Cleanup();
    }
}