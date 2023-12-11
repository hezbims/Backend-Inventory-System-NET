using Inventory_Backend_NET.Constants;
using Inventory_Backend_NET.Utils;
using Microsoft.Extensions.Caching.Distributed;
using NeoSmart.Caching.Sqlite;

namespace Inventory_Backend_NET.Models;

public class Pengajuan
{
    public int Id { get; set; }
    public string KodeTransaksi { get; set; }
    public long CreatedAt { get; set; }
    public Pengaju Pengaju { get; set; }
    public int PengajuId { get; set; }
    public StatusPengajuan Status { get; set; }
    public User User { get; set; }
    public int UserId { get; set; }

    public ICollection<BarangAjuan> BarangAjuans { get; set; } = new List<BarangAjuan>();

    public Pengajuan(
        SqliteCache cache,
        Pengaju pengaju,
        StatusPengajuan status,
        User user,
        ICollection<BarangAjuan> barangAjuans,
        int? id = null
    )
    {
        DateTime currentTime = DateTime.Now;
        CreatedAt = ((DateTimeOffset)currentTime).ToUnixTimeMilliseconds();

        var pengajuanCacheModel = PengajuanCacheModel.From(
            cache.GetString(MyConstants.CacheKeys.UrutanPengajuanHariIni)
        );
        if (!pengajuanCacheModel.Day.IsToday())
        {
            pengajuanCacheModel = new PengajuanCacheModel();
        }

        var tanggalPengajuan = pengajuanCacheModel.Day.ToString("yyyy-MM-dd");
        var urutan = (pengajuanCacheModel.UrutanHari++).ToString().PadLeft(3 , '0');
        cache.SetString(MyConstants.CacheKeys.UrutanPengajuanHariIni, pengajuanCacheModel.ToString());
        
        KodeTransaksi = $"TRX-{tanggalPengajuan}-{urutan}";
        Pengaju = pengaju;
        Status = status;
        User = user;
        BarangAjuans = barangAjuans;
        Id = id ?? default;
    }

    private Pengajuan() { }
}