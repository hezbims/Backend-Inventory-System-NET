using System.ComponentModel.DataAnnotations;
using Inventory_Backend_NET.Database.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Backend_NET.Database.Models;

[EntityTypeConfiguration(typeof(PengajuanConfiguration))]
public class Pengajuan
{
    public int Id { get; set; }
    
    [MaxLength(25)]
    public string KodeTransaksi { get; set; }
    
    // Nyimpen jam dan tanggal kapan pengajuan ini terjadi dalam bentuk millisecond unix timestamp
    public long WaktuPengajuan { get; set; }
    public long WaktuUpdate { get; set; }
    public Pengaju Pengaju { get; set; }
    public int PengajuId { get; set; }
    public StatusPengajuan Status { get; set; }
    public User User { get; set; }
    public int UserId { get; set; }
    

    public ICollection<BarangAjuan> BarangAjuans { get; set; } = new List<BarangAjuan>();

    public Pengajuan(
        Pengaju pengaju,
        StatusPengajuan status,
        User user,
        ICollection<BarangAjuan> barangAjuans,
        long createdAt,
        long updatedAt,
        String kodeTransaksi,
        int? id = null
    )
    {
        WaktuUpdate = updatedAt;
        WaktuPengajuan = createdAt;
        KodeTransaksi = kodeTransaksi;
        Pengaju = pengaju;
        Status = status;
        User = user;
        BarangAjuans = barangAjuans;
        Id = id ?? default;
    }
    

    private Pengajuan() { }
}