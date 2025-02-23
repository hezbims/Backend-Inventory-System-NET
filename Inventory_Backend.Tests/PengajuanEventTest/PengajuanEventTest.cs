using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Inventory_Backend_NET.Fitur.Pengajuan.PengajuanEvent;
using Inventory_Backend.Tests.PostPengajuanTest.Model;
using Inventory_Backend.Tests.TestConfiguration.Constant;
using Inventory_Backend.Tests.TestConfiguration.Fixture;
using Inventory_Backend.Tests.TestData;
using Xunit.Abstractions;

namespace Inventory_Backend.Tests.PengajuanEventTest;

[Collection(TestConstant.IntegrationTestDefinition)]
public class PengajuanEventTest : IDisposable
{
    private readonly TestWebAppFactory _webApp;
    private readonly CompleteTestData _testData;

    public PengajuanEventTest(
        TestWebAppFactory webApp,
        ITestOutputHelper output)
    {
        _webApp = webApp;
        _webApp.ConfigureLoggingToTestOutput(output);
        
        using var db = _webApp.GetDbContext();
        _testData = new CompleteTestSeeder(db: db).Run();
    }

    [Fact]
    public async Task Test_Ketika_Non_Admin_Membuat_Pengajuan_Baru_Maka_Admin_Akan_Menerima_Table_Version_5()
    {
        var nonAdminClient = _webApp.GetAuthorizedClient(isAdmin: false);

        var response = await nonAdminClient.PostAsJsonAsync(
            TestConstant.ApiEndpoints.PostPengajuan,
            new PostPengajuanRequest
            {
                IdPegaju = _testData.Grup.Id,
                ListBarangAjuan =
                [
                    new BarangAjuanRequest
                    {
                        IdBarang = _testData.ListBarang.First().Id,
                        Quantity = 1
                    }
                ]
            });
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var wsClient = _webApp.Server.CreateWebSocketClient();
        var wsUri = new UriBuilder(_webApp.Server.BaseAddress)
        {
            Scheme = "ws",
            Path = "ws/pengajuan/event"
        }.Uri;

        
        var adminWsConnection = await wsClient.ConnectAsync(wsUri, CancellationToken.None);
        await adminWsConnection.SendTextAsync(
            _webApp.GenerateJwt(isAdmin: true));
        var result = int.Parse((await adminWsConnection.ReceiveTextAsync())!);
        
        result.Should().Be(_testData.ListPengajuan.Count + 1);
    }

    public void Dispose()
    {
        _webApp.Cleanup();
    }
}