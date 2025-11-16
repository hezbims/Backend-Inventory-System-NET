using System.Net;
using FluentAssertions;
using Inventory_Backend.Tests.TestConfiguration;
using Inventory_Backend.Tests.TestConfiguration.Constant;
using Inventory_Backend.Tests.TestConfiguration.Fixture;
using Inventory_Backend.Tests.TestData;
using Xunit.Abstractions;

namespace Inventory_Backend.Tests.DeletePengajuanTest;

[Collection(TestConstant.IntegrationTestDefinition)]
public class StockQuantityAfterDeleteTest : BaseIntegrationTest
{
    private readonly CompleteTestData _testData;

    public StockQuantityAfterDeleteTest(
        TestWebAppFactory webApp,
        ITestOutputHelper logger) : base(webApp, logger)
    {
        _testData = new CompleteTestSeeder(db: Db).Run();
    }

    [Fact]
    public async Task Test_Ketika_Delete_Pengajuan_Yang_Statusnya_Diterima_Maka_Stock_Barang_Akan_Dikembalikan()
    {
        var response = await AdminClient.DeleteAsync(
            TestConstant.ApiEndpoints.DeletePengajuan(_testData.ListPengajuan[0].Id));
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        List<int> expectedQuantities = [11, 10, 10, 10, 10];
        var listBarang = Db.Barangs.ToList();

        for (int i = 0; i < listBarang.Count; i++)
        {
            expectedQuantities[i].Should().Be(listBarang[i].CurrentStock);
        }
    }

    [Fact]
    public async Task Test_Ketika_Delete_Pengajuan_Yang_Statusnya_Menunggu_Maka_Stock_Akan_Tetap()
    {
        var response = await NonAdminClient.DeleteAsync(
            TestConstant.ApiEndpoints.DeletePengajuan(_testData.ListPengajuan[2].Id));
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        List<int> expectedQuantities = [10, 10, 10, 10, 10];
        var listBarang = Db.Barangs.ToList();
        for (int i = 0; i < listBarang.Count; i++)
            listBarang[i].CurrentStock.Should().Be(expectedQuantities[i]);
    }

    [Fact]
    public async Task Test_Ketika_Delete_Pengajuan_Yang_Statusnya_Ditolak_Maka_Stock_Akan_Tetap()
    {
        var response = await AdminClient.DeleteAsync(
            TestConstant.ApiEndpoints.DeletePengajuan(_testData.ListPengajuan[2].Id));
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        List<int> expectedQuantities = [10, 10, 10, 10, 10];
        var listBarang = Db.Barangs.ToList();
        for (int i = 0; i < listBarang.Count; i++)
            listBarang[i].CurrentStock.Should().Be(expectedQuantities[i]);
    }
}