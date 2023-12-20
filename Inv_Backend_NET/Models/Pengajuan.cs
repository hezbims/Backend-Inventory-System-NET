using System.ComponentModel.DataAnnotations;
using System.Data;
using Inventory_Backend_NET.Constants;
using Inventory_Backend_NET.Database.Configuration;
using Inventory_Backend_NET.Extension.SqliteCache;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace Inventory_Backend_NET.Models;

[EntityTypeConfiguration(typeof(PengajuanConfiguration))]
public class Pengajuan
{
    public int Id { get; set; }
    
    [MaxLength(25)]
    public string KodeTransaksi { get; set; }
    
    // Nyimpen jam dan tanggal kapan pengajuan ini terjadi dalam bentuk millisecond unix timestamp
    public long WaktuPengajuan { get; set; }
    public Pengaju Pengaju { get; set; }
    public int PengajuId { get; set; }
    public StatusPengajuan Status { get; set; }
    public User User { get; set; }
    public int UserId { get; set; }
    

    public ICollection<BarangAjuan> BarangAjuans { get; set; } = new List<BarangAjuan>();

    public Pengajuan(
        IDistributedCache cache,
        Pengaju pengaju,
        StatusPengajuan status,
        User user,
        ICollection<BarangAjuan> barangAjuans,
        long? createdAt = null,
        int? id = null
    )
    {
        var now = DateTime.Now;
        if (createdAt == null)
            createdAt = ((DateTimeOffset)now).ToUnixTimeMilliseconds();

        WaktuPengajuan = createdAt ?? throw new NoNullAllowedException("createdAt null");

        var tanggalPengajuan = now.ToString("yyyy-MM-dd");
        var tipePengajuan = pengaju.IsPemasok ? "In" : "Out";
        
        // mendapatkan urutan untuk pengajuan ini dari cache
        var urutanHariIni = cache.GetIntValue(MyConstants.CacheKeys.UrutanPengajuanHariIni) + 1;
        var kodeUrutan = urutanHariIni.ToString().PadLeft(3 , '0');
        
        KodeTransaksi = $"TRX-{tipePengajuan}-{tanggalPengajuan}-{kodeUrutan}";
        Pengaju = pengaju;
        Status = status;
        User = user;
        BarangAjuans = barangAjuans;
        Id = id ?? default;
    }
    

    private Pengajuan() { }
}