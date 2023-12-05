using System.Text.Json.Serialization;

namespace Inventory_Backend_NET.DTO.Pengaju;

public class PengajuDto
{
    [JsonPropertyName("id")] public int Id { get; set; }
    
    [JsonPropertyName("nama")] public string Nama { get; set; }

    [JsonPropertyName("is_pemasok")] public bool IsPemasok { get; set; }

    public PengajuDto(int id, string nama, bool isPemasok)
    {
        Id = id;
        Nama = nama;
        IsPemasok = isPemasok;
    }

    public static PengajuDto From(Models.Pengaju pengaju)
    {
        return new PengajuDto(
            id: pengaju.Id,
            nama: pengaju.Nama,
            isPemasok: pengaju.IsPemasok
        );
    }
}