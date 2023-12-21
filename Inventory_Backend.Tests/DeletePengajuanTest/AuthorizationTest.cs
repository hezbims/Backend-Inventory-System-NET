using Inventory_Backend.Tests.TestConfiguration.Constant;
using Inventory_Backend.Tests.TestConfiguration.Fixture;

namespace Inventory_Backend.Tests.DeletePengajuanTest;

// Ngecek apakah pengajuan berhak didelete oleh user atau tidak
[Collection(TestConstant.WithDbCollection)]
public class AuthorizationTest : IDisposable
{
    private readonly MyDbFixture _fixture;

    public AuthorizationTest(MyDbFixture fixture)
    {
        _fixture = fixture;
    }

    public void Test_Ketika_Pengajuannya_Statusnya_Menunggu_Maka_Non_Admin_Maka_Bisa_Delete()
    {
        
    }

    public void Test_Ketika_Pengajuannya_Statusnya_Diterima_Maka_Non_Admin_Tidak_Bisa_Delete()
    {
        
    }

    public void Test_Ketika_Pengajuannya_Statusnya_Diterima_Admin_Tetap_Bisa_Delete()
    {
        
    }

    public void Dispose()
    {
        _fixture.Cleanup();
    }
}