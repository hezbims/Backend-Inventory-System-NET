using System.Text.Json.Serialization;
using Inventory_Backend_NET.DTO.Authentication;
using Inventory_Backend_NET.DTO.Pengaju;

namespace Inventory_Backend_NET.DTO.Pengajuan;

public class PengajuanPreviewDto
{
    [JsonPropertyName("id")] public int Id;

    [JsonPropertyName("pengaju")] public PengajuDto Pengaju;

    [JsonPropertyName("user")] public UserDto User;

    [JsonPropertyName("kode_transaksi")] public string KodeTransaksi;

    [JsonPropertyName("status")] public string Status;

    public PengajuanPreviewDto(int id, PengajuDto pengaju, UserDto user, string kodeTransaksi, string status)
    {
        Id = id;
        Pengaju = pengaju;
        User = user;
        KodeTransaksi = kodeTransaksi;
        this.Status = status;
    }

    public static PengajuanPreviewDto From(Models.Pengajuan pengajuan)
    {
        var dateTime = DateTimeOffset.FromUnixTimeMilliseconds(pengajuan.CreatedAt);
        var kodeTransaksi = dateTime.ToString("yyyy-MM-dd");
        
        return new PengajuanPreviewDto(
            id: pengajuan.Id,
            pengaju: PengajuDto.From(pengajuan.Pengaju),
            user: UserDto.From(pengajuan.User),
            kodeTransaksi: kodeTransaksi,
            status: pengajuan.Status.Value
        );
    }
}