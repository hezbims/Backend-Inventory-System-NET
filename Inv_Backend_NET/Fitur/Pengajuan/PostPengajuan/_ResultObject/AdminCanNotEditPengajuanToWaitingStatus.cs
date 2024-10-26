using System.Net;
using Inventory_Backend_NET.Fitur._Model;

namespace Inventory_Backend_NET.Fitur.Pengajuan.PostPengajuan._ResultObject;

/// <summary>
/// Muncul apabila admin mencoba mengedit pengajuan yang statusnya menunggu dengan memasukkan post body yang statusnya menunggu
/// </summary>
public class AdminCanNotEditPengajuanToWaitingStatus : ResultObject
{
    public string Message => "Admin tidak dapat mengedit pengajuan ke status menunggu. Tolong pilih antara status diterima atau ditolak";
    public string Type => "AdminCanNotEditWaitingPengajuanWithSameStatus";
    public HttpStatusCode StatusCode => HttpStatusCode.Forbidden;
}