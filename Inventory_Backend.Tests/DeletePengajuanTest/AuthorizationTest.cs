using System.Net;
using FluentAssertions;
using Inventory_Backend.Tests.TestConfiguration;
using Inventory_Backend.Tests.TestConfiguration.Constant;
using Inventory_Backend.Tests.TestConfiguration.Fixture;
using Inventory_Backend.Tests.TestData;
using Xunit.Abstractions;

namespace Inventory_Backend.Tests.DeletePengajuanTest;

// Ngecek apakah pengajuan berhak didelete oleh seorang user dengan tipe yang berbeda-beda
[Collection(TestConstant.IntegrationTestDefinition)]
public class AuthorizationTest : BaseIntegrationTest
{
    private readonly CompleteTestData _testData;

    public AuthorizationTest(
        TestWebAppFactory webApp, 
        ITestOutputHelper logger) : base(webApp, logger)
    {
        _testData = new CompleteTestSeeder(db: Db).Run();
    }
    
    [Fact]
    public async Task Test_Ketika_Pengajuannya_Statusnya_Menunggu_Maka_Non_Admin_Maka_Bisa_Delete()
    {
        var response = await NonAdminClient.DeleteAsync(
            TestConstant.ApiEndpoints.DeletePengajuan(_testData.ListPengajuan[2].Id));
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task
        Test_Ketika_Non_Admin_Mencoba_Menghapus_Pengajuan_Orang_Lain_Yang_Statusnya_Menunggu_Maka_Tidak_Bisa_Di_Delete()
    {
        var nonAdminClient2 = GetAuthorizedClient(userId: _testData.NonAdmin2.Id);

        var response = await nonAdminClient2.DeleteAsync(
            TestConstant.ApiEndpoints.DeletePengajuan(_testData.ListPengajuan[2].Id));
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Test_Ketika_Pengajuannya_Statusnya_Diterima_Maka_Non_Admin_Tidak_Bisa_Delete()
    {
        var response = await NonAdminClient.DeleteAsync(
            TestConstant.ApiEndpoints.DeletePengajuan(_testData.ListPengajuan[0].Id));
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Test_Ketika_Pengajuannya_Statusnya_Diterima_Admin_Tetap_Bisa_Delete()
    {
        var response = await AdminClient.DeleteAsync(
            TestConstant.ApiEndpoints.DeletePengajuan(_testData.ListPengajuan[0].Id));
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task Test_Ketika_Pengajuannya_Statusnya_Ditolak_Admin_Tetap_Bisa_Delete()
    {
        var response = await AdminClient.DeleteAsync(
            TestConstant.ApiEndpoints.DeletePengajuan(_testData.ListPengajuan[1].Id));
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}