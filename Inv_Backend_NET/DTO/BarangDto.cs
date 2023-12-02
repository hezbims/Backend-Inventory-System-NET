using System.Text.Json.Serialization;
using Inventory_Backend_NET.Models;

namespace Inventory_Backend_NET.DTO;

public class BarangDto
{
    [JsonPropertyName("id")] 
    public int Id { get; set; }

    [JsonPropertyName("kode_barang")] 
    public string KodeBarang { get; set; } = null!;
    
    [JsonPropertyName("nama")]
    public string Nama { get; set; } = null!;
    
    [JsonPropertyName("kategori")]
    public KategoriDto Kategori{ get; set; } = null!;
    
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
    public string Uom { get; set; } = null!;
    
    public class KategoriDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("nama")]
        public string Nama { get; set; } = null!;
    }

    public static BarangDto From(Barang barang)
    {
        return new BarangDto()
        {
            Id = barang.Id,
            KodeBarang = barang.KodeBarang,
            Nama = barang.Nama,
            Kategori = new KategoriDto()
            {
                Id = barang.Kategori.Id,
                Nama = barang.Kategori.Nama
            },
            MinStock = barang.MinStock,
            NomorRak = barang.NomorRak,
            NomorLaci = barang.NomorLaci,
            NomorKolom = barang.NomorKolom,
            LastMonthStock = barang.LastMonthStock,
            UnitPrice = barang.UnitPrice,
            Uom = barang.Uom
        };
    }
}