using System.ComponentModel.DataAnnotations;
using System.Data;
using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Database.Configuration;
using Microsoft.EntityFrameworkCore;

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
        MyDbContext db,
        Pengaju pengaju,
        StatusPengajuan status,
        User user,
        ICollection<BarangAjuan> barangAjuans,
        TimeProvider timeProvider,
        long? createdAt = null,
        int? id = null
    )
    {

        var now = timeProvider.GetLocalNow();
        if (createdAt == null)
            createdAt = timeProvider.GetLocalNow().ToUnixTimeMilliseconds();

        WaktuPengajuan = createdAt ?? throw new NoNullAllowedException("createdAt null");

        var tanggalPengajuan = now.ToString("yyyy-MM-dd");
        var tipePengajuan = pengaju.IsPemasok ? "IN" : "OUT";
        
        // mendapatkan urutan untuk pengajuan ini dari cache
        var urutanHariIni = (db.TotalPengajuanByTanggals
            .FirstOrDefault(
                t => t.Tanggal == tanggalPengajuan
            )?.Total ?? 0) + 1;
        
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