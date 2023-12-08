using System.ComponentModel.DataAnnotations;

namespace Inventory_Backend_NET.Models;

public class Barang
{
    public int Id { get; set; }
    
    public long CreatedAt { get; set; }
    
    [MaxLength(20)]
    public string KodeBarang { get; set; } = null!;
    
    [MaxLength(100)]
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
    
    [MaxLength(20)]
    public string Uom { get; set; } = null!;
    
    private Barang(){}

    public Barang(
        string nama, 
        int kategoriId, 
        int minStock, 
        int nomorRak, 
        int nomorLaci, 
        int nomorKolom, 
        int currentStock,
        int lastMonthStock, 
        int unitPrice, 
        string uom,
        int? id = null
    )
    {
        Id = id ?? default;
        Nama = nama;
        KategoriId = kategoriId;
        MinStock = minStock;
        NomorRak = nomorRak;
        NomorLaci = nomorLaci;
        NomorKolom = nomorKolom;
        CurrentStock = currentStock;
        LastMonthStock = lastMonthStock;
        UnitPrice = unitPrice;
        Uom = uom;
        KodeBarang = $"R{nomorRak}-{nomorLaci}-{nomorKolom}";
        CreatedAt = ((DateTimeOffset)DateTime.Now).ToUnixTimeMilliseconds();
    }
}