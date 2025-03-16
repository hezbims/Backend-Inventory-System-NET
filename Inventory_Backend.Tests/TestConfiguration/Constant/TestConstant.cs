namespace Inventory_Backend.Tests.TestConfiguration.Constant;

public static class TestConstant
{
    public const string UnitTestWithDbCollection = "Transactional";
    public const string IntegrationTestDefinition = "Integration Test Definition";

    public static class ApiEndpoints
    {
        public const String PostPengajuan = "/api/pengajuan/add";
        public static String DeletePengajuan(int idPengajuan) => $"/api/pengajuan/delete/{idPengajuan}";
        
        public const string GetPengajuansV2 = "/api/pengajuan";
    }
}