using System.Text.Json.Serialization;
using Inventory_Backend_NET.DTO.BarangAjuan;
using Inventory_Backend_NET.DTO.Pengaju;

namespace Inventory_Backend_NET.DTO.Pengajuan;

public class DetailPengajuanDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("created_at")]
    public long CreatedAt { get; set; }
    
    [JsonPropertyName("pengaju")]
    public PengajuDto Pengaju { get; set; }
    
    [JsonPropertyName("list_barang")]
    public ICollection<BarangAjuanDto> BarangAjuans { get; set; }
    
    [JsonPropertyName("status")]
    public string Status { get; set; }

    private DetailPengajuanDto(
        int id, 
        long createdAt, 
        PengajuDto pengaju, 
        ICollection<BarangAjuanDto> barangAjuans, 
        string status
    )
    {
        Id = id;
        CreatedAt = createdAt;
        Pengaju = pengaju;
        BarangAjuans = barangAjuans;
        Status = status;
    }

    public static DetailPengajuanDto From(Models.Pengajuan pengajuan)
    {
        return new DetailPengajuanDto(
            id: pengajuan.Id,
            createdAt: pengajuan.CreatedAt,
            pengaju: PengajuDto.From(pengajuan.Pengaju),
            status: pengajuan.Status.Value,
            barangAjuans: pengajuan.BarangAjuans.Select(
                barangAjuan => 
                    BarangAjuanDto.From(barangAjuan)
            ).ToList()
        );
    }
}