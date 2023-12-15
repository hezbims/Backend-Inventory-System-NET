using System.Text.Json.Serialization;

namespace Inventory_Backend_NET.DTO.Barang;

public class BarangDto
{
    [JsonPropertyName("id")] 
    public int Id { get; set; }

    [JsonPropertyName("kode_barang")] 
    public string KodeBarang { get; set; }
    
    [JsonPropertyName("nama")]
    public string Nama { get; set; }
    
    [JsonPropertyName("kategori")]
    public KategoriDto Kategori{ get; set; }
    
    [JsonPropertyName("min_stock")]
    public int MinStock { get; set; }
    
    [JsonPropertyName("nomor_rak")]
    public int NomorRak { get; set;  }
    
    [JsonPropertyName("nomor_laci")]
    public int NomorLaci { get; set; }
    
    [JsonPropertyName("nomor_kolom")]
    public int NomorKolom { get; set; }
    
    [JsonPropertyName("stock_sekarang")]
    public int CurrentStock { get; set; }
    
    [JsonPropertyName("last_month_stock")]
    public int LastMonthStock { get; set;  }
    
    [JsonPropertyName("unit_price")]
    public int UnitPrice { get; set;  }
    
    [JsonPropertyName("uom")]
    public string Uom { get; set; }

    private BarangDto()
    {
    }

    public BarangDto(
        int id, 
        string kodeBarang, 
        string nama, 
        KategoriDto kategori, 
        int minStock, 
        int nomorRak, 
        int nomorLaci, 
        int nomorKolom, 
        int currentStock, 
        int lastMonthStock, 
        int unitPrice, 
        string uom
    )
    {
        Id = id;
        KodeBarang = kodeBarang;
        Nama = nama;
        Kategori = kategori;
        MinStock = minStock;
        NomorRak = nomorRak;
        NomorLaci = nomorLaci;
        NomorKolom = nomorKolom;
        CurrentStock = currentStock;
        LastMonthStock = lastMonthStock;
        UnitPrice = unitPrice;
        Uom = uom;
    }

    public static BarangDto From(Models.Barang barang)
    {
        return new BarangDto
        (
            id: barang.Id,
            kodeBarang: barang.KodeBarang,
            nama: barang.Nama,
            kategori: KategoriDto.From(barang.Kategori),
            minStock: barang.MinStock,
            nomorRak: barang.NomorRak,
            nomorLaci: barang.NomorLaci,
            nomorKolom: barang.NomorKolom,
            currentStock: barang.CurrentStock,
            lastMonthStock: barang.LastMonthStock,
            unitPrice: barang.UnitPrice,
            uom: barang.Uom
        );
    }
}