using System.Text.Json.Serialization;

namespace Inventory_Backend_NET.Fitur.Laporan.GetLaporan._Dto.Response;

public class GetLaporanDto
{
    [JsonPropertyName("nama")]
    public string NamaKategori { get; set; }

    [JsonPropertyName("barang")] 
    public ICollection<BarangLaporanDto> Barangs { get; set; }
}

public class BarangLaporanDto
{
    [JsonPropertyName("id")] 
    public int Id { get; set; }

    [JsonPropertyName("nomor_rak")] 
    public int NomorRak { get; set; }

    [JsonPropertyName("nomor_laci")] 
    public int NomorLaci { get; set; }

    [JsonPropertyName("nomor_kolom")] 
    public int NomorKolom { get; set; }
    
    [JsonPropertyName("nama")]
    public string Nama { get; set; }
    
    [JsonPropertyName("uom")]
    public string Uom { get; set; }
    
    [JsonPropertyName("min_stock")]
    public int MinStock { get; set; }
    
    [JsonPropertyName("last_month_stock")]
    public int LastMonthStock {
        get => StockSekarang + In - Out; 
    }
    
    [JsonPropertyName("in")]
    public int In { get; set; }
    
    [JsonPropertyName("out")]
    public int Out { get; set; }
    
    [JsonPropertyName("stock_sekarang")]
    public int StockSekarang { get; set; }
    
    [JsonPropertyName("unit_price")]
    public int UnitPrice { get; set; }
    
    [JsonPropertyName("amount")]
    public int Amount {
        get => UnitPrice * StockSekarang;
    }

    [JsonPropertyName("kode_barang")] 
    public string KodeBarang { get; set; }
    
    public int KategoriId { get; set; }
}