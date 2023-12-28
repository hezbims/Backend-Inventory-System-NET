namespace Inventory_Backend_NET.Database.Models;

public class BarangAjuan
{
    public int Id { get; set;  }
    public Pengajuan Pengajuan { get; set; } = null!;
    public int PengajuanId { get; set; }
    public Barang Barang { get; set; } = null!;
    public int BarangId { get; set; }
    public int Quantity { get; set;  }
    public string? Keterangan { get; set; }
    
    private BarangAjuan(){}

    public BarangAjuan(
        Barang barang,
        int quantity,
        string? keterangan
    )
    {
        Barang = barang;
        Quantity = quantity;
        Keterangan = keterangan;
    }
    
    public BarangAjuan(
        int barangId,
        int quantity,
        string? keterangan
    )
    {
        BarangId = barangId;
        Quantity = quantity;
        Keterangan = keterangan;
    }
}