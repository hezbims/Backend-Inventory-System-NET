using System.Net;
using Inventory_Backend_NET.Database.Models;
using Inventory_Backend.Tests.Seeder;
using Inventory_Backend.Tests.TestConfiguration;
using Inventory_Backend.Tests.TestConfiguration.Constant;
using Inventory_Backend.Tests.TestConfiguration.Fixture;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace Inventory_Backend.Tests.Fitur.Product.PostProductCsvTest.CategoryNameSame;

[Collection(TestConstant.IntegrationTestDefinition)]
public class CategoryNameSameTests : BaseIntegrationTest
{
    public CategoryNameSameTests(TestWebAppFactory webApp, ITestOutputHelper output) : base(webApp, output)
    {
        Get<UserSeeder>().CreateAdmin();
    }

    [Fact]
    public async Task category_should_using_previous_instance_if_already_exist()
    {
        using var requestBody = new MultipartFormDataContent();
        using var csvStream = TestAssetsUtils.GetFileStream(
            "./Fitur/Product/PostProductCsvTest/CategoryNameSame/_Preparation/has_overlapping_category.csv");
        
        requestBody.Add(csvStream, "csv", "file.csv");
        requestBody.Add(new StringContent("false"), "overwrite_by_kode_barang");

        var response = await AdminClient.PostAsync(
            TestConstant.ApiEndpoints.Product.PostCsv, requestBody);
        Output.WriteLine(await response.Content.ReadAsStringAsync());
        
        List<Barang> products = Db.Barangs
            .Include(barang => barang.Kategori)
            .ToList();
        List<Kategori> categories = Db.Kategoris.ToList();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(3, products.Count);
        Assert.Equal(2, categories.Count);
        Assert.Equal(products.First().KategoriId, products.Last().KategoriId);
        Assert.Equal(products.First().Kategori.Nama, products.Last().Kategori.Nama);
    }
}