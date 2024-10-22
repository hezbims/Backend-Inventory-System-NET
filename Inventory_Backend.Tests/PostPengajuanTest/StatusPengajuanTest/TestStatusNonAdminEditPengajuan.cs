using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Inventory_Backend.Tests.Model;
using Inventory_Backend.Tests.PostPengajuanTest.Model;
using Inventory_Backend.Tests.TestConfiguration.Constant;
using Inventory_Backend.Tests.TestConfiguration.Fixture;
using Inventory_Backend.Tests.TestData;
using Xunit.Abstractions;

namespace Inventory_Backend.Tests.PostPengajuanTest.StatusPengajuanTest;

[Collection(TestConstant.IntegrationTestDefinition)]
public class TestStatusNonAdminEditPengajuan : IDisposable
{
    private readonly TestWebAppFactory _webApp;
    private readonly HttpClient _client;
    private readonly CompleteTestData _testData;

    public TestStatusNonAdminEditPengajuan(
        TestWebAppFactory webApp,
        ITestOutputHelper logger)
    {
        _webApp = webApp;
        _webApp.ConfigureLoggingToTestOutput(logger);

        using var db = _webApp.GetDbContext();
        _testData = new CompleteTestSeeder(db: db).Run();
        _client = _webApp.GetAuthorizedClient(isAdmin: false);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public async Task Test_Non_Admin_Edit_Pengajuan_Yang_Tidak_Menunggu_Maka_Response_Dari_Server_Forbidden(
        int pengajuanIndex)
    {
        var previousPengajuan = _testData.ListPengajuan[pengajuanIndex];

        var response = await _client.PostAsJsonAsync(
            TestConstant.ApiEndpoints.PostPengajuan,
            new PostPengajuanRequest
            {
                IdPengajuan = previousPengajuan.Id,
                IdPegaju = previousPengajuan.Pengaju.Id,
                ListBarangAjuan = [
                    new BarangAjuanRequest
                    {
                        IdBarang = _testData.ListBarang.First().Id,
                        Quantity = 1
                    }
                ]
            });
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);

        var responseModel = await TestResponseModel.From(response);
        responseModel.Type.Should().Be("NonAdminCanNotEditAcceptedOrRejectedPengajuan");
    }

    public void Dispose()
    {
        _webApp.Cleanup();
    }
}