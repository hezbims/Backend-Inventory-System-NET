namespace Inventory_Backend_NET.Constants;

public class MyConstants
{
    public static int PageSize  = 15;
    public class Roles
    {
        public const string Admin = "Admin";
        public const string NonAdmin = "NonAdmin";
    }
    public class Policies
    {
        public const string AllUsers = "All-Users";
        public const string AdminOnly = "Admin-Only";
        public const string CorsPolicy = "Cors-Policy";
    }
    public class CacheKeys
    {
        public const string UrutanPengajuanHariIni = "urutan-pengajuan";
    }

    public class AppSettingsKey
    {
        public const string MyConnectionString = "SQLServerConnection";
    }
}