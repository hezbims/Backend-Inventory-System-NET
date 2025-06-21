using System.Net;
using Inventory_Backend_NET.Database.Models;
using Inventory_Backend.Tests.Seeder;
using Inventory_Backend.Tests.TestConfiguration;
using Inventory_Backend.Tests.TestConfiguration.Constant;
using Inventory_Backend.Tests.TestConfiguration.Fixture;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace Inventory_Backend.Tests.Fitur.Product.PostProductCsvTest.OverwriteByProductCode;

/// <summary>
/// Memastikan bisa mencegah atau menimpa data product dengan kode yang sama
/// </summary>
[Collection(TestConstant.IntegrationTestDefinition)]
public class OverwriteProductByCodeTests : BaseIntegrationTest
{
    public OverwriteProductByCodeTests(TestWebAppFactory factory, ITestOutputHelper output) : 
        base(factory, output)
    {
        factory.Get<UserSeeder>().CreateAdmin();
    }

    [Fact]
    public async Task test_allow_override_existing_product()
    {
        using var requestBody = new MultipartFormDataContent();
        using var csvStream = TestAssetsUtils.GetFileStream(
            "./Fitur/Product/PostProductCsvTest/OverwriteByProductCode/_Preparation/two_product_with_same_code.csv");
        
        requestBody.Add(csvStream, "csv", "file.csv");
        requestBody.Add(new StringContent("true"), "overwrite_by_kode_barang");

        var response = await AdminClient.PostAsync(
            TestConstant.ApiEndpoints.Product.PostCsv, requestBody);
        Output.WriteLine(await response.Content.ReadAsStringAsync());
        
        Barang product = Db.Barangs
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
        using var requestBody = new MultipartFormDataContent();
        using var csvStream = TestAssetsUtils.GetFileStream(
            "./Fitur/Product/PostProductCsvTest/OverwriteByProductCode/_Preparation/two_product_with_same_code.csv");
        
        requestBody.Add(csvStream, "csv", "file.csv");
        requestBody.Add(new StringContent("false"), "overwrite_by_kode_barang");

        var response = await AdminClient.PostAsync(
            TestConstant.ApiEndpoints.Product.PostCsv, requestBody);
        
        Output.WriteLine(await response.Content.ReadAsStringAsync());
        
        Assert.Empty(Db.Barangs.ToList());
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}