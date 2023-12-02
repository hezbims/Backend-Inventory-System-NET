namespace Inventory_Backend_NET.Models;

public class Barang
{
    public int Id { get; set; }
    public string KodeBarang { get; set; } = null!;
    public string Nama { get; set; } = null!;
    public Kategori Kategori { get; set; } = null!;
    public int KategoriId { get; set; }
    
    public int MinStock { get; set; }
    public int NomorRak { get; set;  }
    public int NomorLaci { get; set; }
    public int NomorKolom { get; set; }
    public int CurrentStock { get; set; }
    public int LastMonthStock { get; set;  }
    public int UnitPrice { get; set;  }
    public string Uom { get; set; } = null!;
}