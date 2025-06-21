using System.Net;
using Inventory_Backend.Tests.Fitur.Product.PostProductCsvTest._Dto;
using Inventory_Backend.Tests.Seeder;
using Inventory_Backend.Tests.TestConfiguration;
using Inventory_Backend.Tests.TestConfiguration.Constant;
using Inventory_Backend.Tests.TestConfiguration.Fixture;
using Newtonsoft.Json;
using Xunit.Abstractions;

namespace Inventory_Backend.Tests.Fitur.Product.PostProductCsvTest.InvalidRowData;

using User = Inventory_Backend_NET.Database.Models.User;

[Collection(TestConstant.IntegrationTestDefinition)]
public class InvalidRowDataTests
    : BaseIntegrationTest
{
    public InvalidRowDataTests(TestWebAppFactory webApp, ITestOutputHelper output) : base(webApp, output)
    {
        Get<UserSeeder>().CreateAdmin();
    }
    
    [Fact]
    public async Task should_display_correct_error_when_row_data_is_invalid()
    {
        using var requestBody = new MultipartFormDataContent();
        using var csvStream = TestAssetsUtils.GetFileStream(
            "./Fitur/Product/PostProductCsvTest/InvalidRowData/_Preparation/invalid_field.csv");
        
        requestBody.Add(csvStream, "csv", "file.csv");
        requestBody.Add(new StringContent("true"), "overwrite_by_kode_barang");

        var response = await AdminClient.PostAsync(
            TestConstant.ApiEndpoints.Product.PostCsv, requestBody);
        string responseBody = await response.Content.ReadAsStringAsync();
        PostProductCsvErrorDto errorDto = JsonConvert.DeserializeObject<PostProductCsvErrorDto>(responseBody)!;
        
        Output.WriteLine(responseBody);
        
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Empty(Db.Barangs.ToList());
        Assert.Empty(Db.Kategoris.ToList());
        var baris3 = errorDto.Errors["BARIS #3"];
        Assert.Contains("Kode Barang tidak boleh kosong", baris3);
        Assert.Contains("Nama Barang tidak boleh kosong", baris3);
        Assert.Contains("Kategori tidak boleh kosong", baris3);
        Assert.Contains("Nomor Rak harus di rentang 1-6", baris3);
        Assert.Contains("Nomor Laci harus di rentang 1-30" , baris3);
        Assert.Contains("Nomor Kolom harus di rentang 1-9", baris3);
        Assert.Equal(6 , baris3.Count);

        var baris4 = errorDto.Errors["BARIS #4"];
        Assert.Contains("Nomor Kolom harus di rentang 1-9", baris4);
        Assert.Contains("Current Stock tidak valid", baris4);
        Assert.Contains("Min. Stock tidak valid", baris4);
        Assert.Contains("Last Month Stock tidak valid", baris4);
        Assert.Contains("Unit Price tidak valid", baris4);
        Assert.Contains("UOM tidak boleh kosong", baris4);
        Assert.Equal(6 , baris4.Count);
    }
}