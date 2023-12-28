using System.Text.Json.Serialization;
using Inventory_Backend_NET.Fitur._Dto.Response;
using Inventory_Backend_NET.Fitur.Autentikasi._Dto.Response;

namespace Inventory_Backend_NET.Fitur.Pengajuan.GetPengajuanPaginated._Dto.Response;

public class PengajuanPreviewDto
{
    [JsonPropertyName("id")] public int Id {get; set;}

    [JsonPropertyName("pengaju")] public PengajuDto Pengaju {get; set;}

    [JsonPropertyName("user")] public UserDto User {get; set;}

    [JsonPropertyName("kode_transaksi")] public string KodeTransaksi {get; set;}

    [JsonPropertyName("status")] public string Status {get; set;}

    public PengajuanPreviewDto(int id, PengajuDto pengaju, UserDto user, string kodeTransaksi, string status)
    {
        Id = id;
        Pengaju = pengaju;
        User = user;
        KodeTransaksi = kodeTransaksi;
        this.Status = status;
    }

    public static PengajuanPreviewDto From(Database.Models.Pengajuan pengajuan)
    {
        return new PengajuanPreviewDto(
            id: pengajuan.Id,
            pengaju: PengajuDto.From(pengajuan.Pengaju),
            user: UserDto.From(pengajuan.User),
            kodeTransaksi: pengajuan.KodeTransaksi,
            status: pengajuan.Status.Value
        );
    }
}