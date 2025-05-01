using System.Net;
using Inventory_Backend_NET.Database.Models;
using Inventory_Backend.Tests.TestConfiguration;
using Inventory_Backend.Tests.TestConfiguration.CollectionDefinition;
using Inventory_Backend.Tests.TestConfiguration.Constant;
using Inventory_Backend.Tests.TestConfiguration.Fixture;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace Inventory_Backend.Tests.Fitur.Product.PostProductCsvTest;

[Collection(TestConstant.IntegrationTestDefinition)]
public class OverwriteProductByCodeTests : IDisposable
{
    private readonly TestWebAppFactory _webAppFactory;
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _output;
    
    public OverwriteProductByCodeTests(
        TestWebAppFactory factory,
        ITestOutputHelper output)
    {
        _webAppFactory = factory;
        _webAppFactory.ConfigureLoggingToTestOutput(output);
        _output = output;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task test_allow_override_existing_product()
    {
        await using var db = _webAppFactory.GetDbContext();
        using var requestBody = new MultipartFormDataContent();
        using var csvStream = TestAssetsUtils.GetFileStream(
            "Fitur/Product/_Preparation/Assets/two_product_with_same_code.csv");
        
        requestBody.Add(csvStream, "csv", "file.csv");
        requestBody.Add(new StringContent("true"), "overwrite_by_kode_barang");

        var response = await _client.PostAsync(
            TestConstant.ApiEndpoints.Product.PostCsv, requestBody);
        _output.WriteLine(await response.Content.ReadAsStringAsync());
        
        Barang product = db.Barangs
            .Include(barang => barang.Kategori)
            .Single();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(product.Id > 0);
        Assert.Equal("R1-1-1", product.KodeBarang);
        Assert.Equal("Oli", product.Nama);
        Assert.Equal("Oli", product.Kategori.Nama);
        Assert.Equal(1, product.NomorRak);
        Assert.Equal(1, product.NomorLaci);
        Assert.Equal(1, product.NomorKolom);
        Assert.Equal(5, product.CurrentStock);
        Assert.Equal(1, product.MinStock);
        Assert.Equal(5, product.LastMonthStock);
        Assert.Equal(5000, product.UnitPrice);
        Assert.Equal("Liter", product.Uom);
    }

    [Fact]
    public async Task test_prevent_overwrite_existing_product()
    {
        await using var db = _webAppFactory.GetDbContext();
        using var requestBody = new MultipartFormDataContent();
        using var csvStream = TestAssetsUtils.GetFileStream(
            "Fitur/Product/_Preparation/Assets/two_product_with_same_code.csv");
        
        requestBody.Add(csvStream, "csv", "file.csv");
        requestBody.Add(new StringContent("false"), "overwrite_by_kode_barang");

        var response = await _client.PostAsync(
            TestConstant.ApiEndpoints.Product.PostCsv, requestBody);
        
        _output.WriteLine(await response.Content.ReadAsStringAsync());
        
        Assert.Empty(db.Barangs.ToList());
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    public void Dispose()
    {
        _client.Dispose();
        _webAppFactory.Cleanup();
    }
}