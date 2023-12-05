using Inventory_Backend_NET.Constants;
using Inventory_Backend_NET.Utils;
using Microsoft.Extensions.Caching.Distributed;
using NeoSmart.Caching.Sqlite;

namespace Inventory_Backend_NET.Models;

public class Pengajuan
{
    public int Id { get; set; }
    public long CreatedAt { get; set; }
    public Pengaju Pengaju { get; set; } = null!;
    public int PengajuId { get; set; }
    public int UrutanHariIni { get; set; }
    public StatusPengajuan Status { get; set; } = null!;
    public User User { get; set; } = null!;
    public int UserId { get; set; }

    public ICollection<BarangAjuan> BarangAjuans { get; set; } = new List<BarangAjuan>();

    
    public Pengajuan(SqliteCache cache)
    {
        DateTime currentTime = DateTime.Now;
        CreatedAt = ((DateTimeOffset)currentTime).ToUnixTimeMilliseconds();

        var urutanPengajuan = UrutanPengajuanCacheModel.From(cache.GetString(CacheKeys.UrutanPengajuanHariIni));
        if (!urutanPengajuan.Day.IsToday())
        {
            urutanPengajuan = new UrutanPengajuanCacheModel();
        }

        UrutanHariIni = urutanPengajuan.UrutanHari++;
        cache.SetString(CacheKeys.UrutanPengajuanHariIni, urutanPengajuan.ToString());
    }
    
    public Pengajuan(){}
}