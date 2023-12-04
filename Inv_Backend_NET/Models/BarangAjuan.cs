namespace Inventory_Backend_NET.Models;

public class BarangAjuan
{
    public int Id { get; set;  }
    public Pengajuan Pengajuan { get; set; } = null!;
    public int PengajuanId { get; set; }
    public Barang Barang { get; set; } = null!;
    public int BarangId { get; set; }
    public int Quantity { get; set;  }
    public string Keterangan { get; set; } = null!;
}