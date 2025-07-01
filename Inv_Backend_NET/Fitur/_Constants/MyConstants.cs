using Inventory_Backend_NET.Database.Models;

namespace Inventory_Backend_NET.Fitur._Constants;

public static class MyConstants
{
    public const int DefaultPageSize  = 50;
    public static class Roles
    {
        public const string Admin = "Admin";
        public const string NonAdmin = "NonAdmin";
    }
    public static class Policies
    {
        public const string AllUsers = "All-Users";
        public const string AdminOnly = "Admin-Only";
        public const string CorsPolicy = "Cors-Policy";
    }
    public static class CacheKeys
    {
        public static string PengajuanTableVersionByUser(User user)
        {
            return $"pengajuan_table_version : {user.Id}";
        }
        
        public const string PengajuanTableVersionForAdmin = "pengajuan_table_version_for_admin";
    }

    public static class AppSettingsKey
    {
        public const string MyConnectionString = "SQLServerConnection";
    }
    
    public const string DateFormat = "yyyy-MM-dd";
    
    /// <summary>
    /// Key untuk error yang diakibatkan karena aplikasi FE gagal
    /// memastikan input yang dikirimkan ke BE terformat dengan benar
    /// (bukan salah input dari user)
    /// </summary>
    public const string WrongContractErrorKey = "__wrong_contract__";
}