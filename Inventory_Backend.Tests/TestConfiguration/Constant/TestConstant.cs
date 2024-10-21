namespace Inventory_Backend.Tests.TestConfiguration.Constant;

public static class TestConstant
{
    public const string UnitTestWithDbCollection = "Transactional";
    public const string IntegrationTestDefinition = "Integration Test Definition";

    public static class ApiEndpoints
    {
        public static String DeletePengajuan(int idPengajuan) => $"/api/pengajuan/delete/{idPengajuan}";
    }
}