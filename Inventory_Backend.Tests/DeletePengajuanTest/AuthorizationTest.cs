using System.Net;
using FluentAssertions;
using Inventory_Backend.Tests.TestConfiguration.Constant;
using Inventory_Backend.Tests.TestConfiguration.Fixture;
using Inventory_Backend.Tests.TestData;
using Xunit.Abstractions;

namespace Inventory_Backend.Tests.DeletePengajuanTest;

// Ngecek apakah pengajuan berhak didelete oleh seorang user dengan tipe yang berbeda-beda
[Collection(TestConstant.IntegrationTestDefinition)]
public class AuthorizationTest : IDisposable
{
    private readonly TestWebAppFactory _webApp;
    private readonly CompleteTestData _testData;

    public AuthorizationTest(
        TestWebAppFactory webApp, 
        ITestOutputHelper logger)
    {
        _webApp = webApp;
        _webApp.ConfigureLoggingToTestOutput(logger);

        using var db = _webApp.GetDbContext();
        _testData = new CompleteTestSeeder(db: db).Run();
    }
    
    [Fact]
    public async Task Test_Ketika_Pengajuannya_Statusnya_Menunggu_Maka_Non_Admin_Maka_Bisa_Delete()
    {
        var nonAdminClient = _webApp.GetAuthorizedClient(isAdmin: false);
        var response = await nonAdminClient.DeleteAsync(
            TestConstant.ApiEndpoints.DeletePengajuan(_testData.ListPengajuan[2].Id));
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task
        Test_Ketika_Non_Admin_Mencoba_Menghapus_Pengajuan_Orang_Lain_Yang_Statusnya_Menunggu_Maka_Tidak_Bisa_Di_Delete()
    {
        var nonAdminClient2 = _webApp.GetAuthorizedClient(userId: _testData.NonAdmin2.Id);

        var response = await nonAdminClient2.DeleteAsync(
            TestConstant.ApiEndpoints.DeletePengajuan(_testData.ListPengajuan[2].Id));
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    public void Test_Ketika_Pengajuannya_Statusnya_Diterima_Maka_Non_Admin_Tidak_Bisa_Delete()
    {
        
    }

    public void Test_Ketika_Pengajuannya_Statusnya_Diterima_Admin_Tetap_Bisa_Delete()
    {
        
    }

    public void Dispose()
    {
        _webApp.Cleanup();
    }
}