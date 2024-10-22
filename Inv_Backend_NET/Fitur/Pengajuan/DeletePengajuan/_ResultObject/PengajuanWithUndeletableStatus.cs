using System.Net;
using Inventory_Backend_NET.Fitur._Model;

namespace Inventory_Backend_NET.Fitur.Pengajuan.DeletePengajuan._ResultObject;

/// <summary>
/// Keluar ketika pengguna non-admin mencoba menghapus pengajuan yang sudah diterima atau ditolak.
/// </summary>
public class PengajuanWithUndeletableStatus : ResultObject
{
    public string Message => "Tidak dapat menghapus pengajuan yang sudah diterima atau ditolak";
    public string Type => "PengajuanWithUndeletableStatus";
    public HttpStatusCode StatusCode => HttpStatusCode.Forbidden;
}