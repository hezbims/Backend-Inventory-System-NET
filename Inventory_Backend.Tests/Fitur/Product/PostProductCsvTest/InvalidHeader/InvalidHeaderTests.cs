using System.Net;
using Inventory_Backend.Tests.Fitur.Product.PostProductCsvTest._Dto;
using Inventory_Backend.Tests.Seeder;
using Inventory_Backend.Tests.TestConfiguration;
using Inventory_Backend.Tests.TestConfiguration.Constant;
using Inventory_Backend.Tests.TestConfiguration.Fixture;
using Newtonsoft.Json;
using Xunit.Abstractions;

namespace Inventory_Backend.Tests.Fitur.Product.PostProductCsvTest.InvalidHeader;

[Collection(TestConstant.IntegrationTestDefinition)]
public class InvalidHeaderTests : BaseIntegrationTest
{
    
    public InvalidHeaderTests(TestWebAppFactory factory, ITestOutputHelper output) : base(factory, output)
    {
        factory.Get<UserSeeder>().CreateAdmin();
    }

    [Fact]
    public async Task should_display_error_correctly_when_there_is_no_header()
    {
        using var requestBody = new MultipartFormDataContent();
        using var csvStream = TestAssetsUtils.GetFileStream(
            "./Fitur/Product/PostProductCsvTest/InvalidHeader/_Preparation/no_headers.csv");
        
        requestBody.Add(csvStream, "csv", "file.csv");
        requestBody.Add(new StringContent("false"), "overwrite_by_kode_barang");

        var response = await AdminClient.PostAsync(
            TestConstant.ApiEndpoints.Product.PostCsv, requestBody);
        Output.WriteLine(await response.Content.ReadAsStringAsync());

        string responseBody = await response.Content.ReadAsStringAsync();
        PostProductCsvErrorDto dto = JsonConvert.DeserializeObject<PostProductCsvErrorDto>(responseBody)!;
        List<String> error = dto.Errors.Single().Value;
        
        Assert.Empty(Db.Barangs.ToList());
        Assert.Empty(Db.Kategoris.ToList());
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("Header 'KODE BARANG, NAMA BARANG, KATEGORI, NOMOR RAK, NOMOR LACI, NOMOR KOLOM, CURRENT STOCK, MIN. STOCK, LAST MONTH STOCK, UNIT PRICE, UOM' tidak ditemukan dalam CSV", error.Single());        
    }
    
    [Fact]
    public async Task should_display_error_correctly_when_there_is_partially_incorrect_headers()
    {
        using var requestBody = new MultipartFormDataContent();
        using var csvStream = TestAssetsUtils.GetFileStream(
            "./Fitur/Product/PostProductCsvTest/InvalidHeader/_Preparation/partially_incorrect_headers.csv");
        
        requestBody.Add(csvStream, "csv", "file.csv");
        requestBody.Add(new StringContent("true"), "overwrite_by_kode_barang");

        var response = await AdminClient.PostAsync(
            TestConstant.ApiEndpoints.Product.PostCsv, requestBody);
        Output.WriteLine(await response.Content.ReadAsStringAsync());

        string responseBody = await response.Content.ReadAsStringAsync();
        PostProductCsvErrorDto dto = JsonConvert.DeserializeObject<PostProductCsvErrorDto>(responseBody)!;
        List<String> error = dto.Errors.Single().Value;
        
        Assert.Empty(Db.Barangs.ToList());
        Assert.Empty(Db.Kategoris.ToList());
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("Header 'KODE BARANG, NAMA BARANG, CURRENT STOCK, LAST MONTH STOCK' tidak ditemukan dalam CSV", error.Single());
    }
}