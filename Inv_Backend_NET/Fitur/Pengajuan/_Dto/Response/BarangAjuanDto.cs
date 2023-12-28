using System.Text.Json.Serialization;

namespace Inventory_Backend_NET.Fitur.Pengajuan._Dto.Response;

public class BarangAjuanDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }
    
    [JsonPropertyName("keterangan")]
    public string? Keterangan { get; set; }
    
    [JsonPropertyName("barang")] 
    public BarangWithOnlyNamaDto Barang { get; set; }

    private BarangAjuanDto(int id, int quantity, string? keterangan, BarangWithOnlyNamaDto barang)
    {
        Id = id;
        Quantity = quantity;
        Keterangan = keterangan;
        Barang = barang;
    }

    public static BarangAjuanDto From(Database.Models.BarangAjuan barangAjuan)
    {
        return new BarangAjuanDto(
            id: barangAjuan.Id,
            quantity: barangAjuan.Quantity,
            keterangan: barangAjuan.Keterangan,
            barang: BarangWithOnlyNamaDto.From(barang: barangAjuan.Barang)
        );
    }
}

public class BarangWithOnlyNamaDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("nama")] 
    public string Nama { get; set; }

    private BarangWithOnlyNamaDto(int id, string nama)
    {
        Id = id;
        Nama = nama;
    }
    
    public static BarangWithOnlyNamaDto From(Database.Models.Barang barang)
    {
        return new BarangWithOnlyNamaDto(
            nama: barang.Nama,
            id: barang.Id
        );
    }
}