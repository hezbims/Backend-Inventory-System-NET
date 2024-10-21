using System.Net;
using Inventory_Backend_NET.Fitur._Model;

namespace Inventory_Backend_NET.Fitur.Pengajuan._Model.Exception;

/// <summary>
/// Keluar ketika user biasa mencoba menghapus pengajuan yang bukan miliknya. Hanya user admin yang dapat menghapus pengajuan yang bukan miliknya.
/// </summary>
public class NotOwnedDeletedPengajuan : ResultObject
{
    public string Message => "Tidak dapat menghapus pengajuan. Pengajuan bukan milik anda.";
    public string Type => "NotOwnedDeletedPengajuan";
    public HttpStatusCode StatusCode => HttpStatusCode.Unauthorized;
}