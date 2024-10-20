using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Inventory_Backend.Tests.PostPengajuanTest.Model;
using Inventory_Backend.Tests.TestConfiguration.Constant;
using Inventory_Backend.Tests.TestConfiguration.Fixture;
using Inventory_Backend.Tests.TestData;
using Xunit.Abstractions;

namespace Inventory_Backend.Tests.PostPengajuanTest.StockQuantityTest;

[Collection(TestConstant.IntegrationTestDefinition)]
public class EditPengajuanStockQuantityTest : IDisposable
{
    private readonly TestWebAppFactory _webApp;
    private readonly CompleteTestData _testData;

    public EditPengajuanStockQuantityTest(
        TestWebAppFactory webApp,
        ITestOutputHelper logger)
    {
        _webApp = webApp;
        _webApp.ConfigureLoggingToTestOutput(logger);
        
        using var db = _webApp.GetDbContext();
        _testData = new CompleteTestSeeder(db: db).Run();
    }

    [Fact]
    public async Task Test_Admin_Terima_Pengajuan_Maka_Stock_Terupdate_Dengan_Benar()
    {
        var adminClient = _webApp.GetAuthorizedClient(userId: _testData.NonAdmin.Id);

        var previousPengajuan = _testData.ListPengajuan.Last();
        var response = await adminClient.PostAsJsonAsync(
            "api/pengajuan/add",
            new CreatePengajuanRequest
            {
                IdPengajuan = previousPengajuan.Id,
                IdPegaju = previousPengajuan.PengajuId,
                ListBarangAjuan = [
                    new BarangAjuanRequest
                    {
                        Quantity = 2,
                        IdBarang = previousPengajuan.BarangAjuans.Single().BarangId
                    },
                    new BarangAjuanRequest
                    {
                        Quantity = 3,
                        IdBarang = _testData.ListBarang[1].Id
                    }
                ]
            });

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await using var db = _webApp.GetDbContext();
        var newBarang = db.Barangs.ToList();
        List<int> expectedQuantities = [9, 7, 10, 10, 10];

        for (int i = 0; i < expectedQuantities.Count; i++)
        {
            newBarang[i].CurrentStock.Should().Be(expectedQuantities[i]);
        }
    }

    public void Dispose()
    {
        _webApp.Cleanup();
    }
}