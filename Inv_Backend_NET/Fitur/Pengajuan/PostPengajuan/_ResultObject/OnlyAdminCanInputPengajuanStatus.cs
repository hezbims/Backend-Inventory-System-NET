using System.Net;
using Inventory_Backend_NET.Fitur._Model;

namespace Inventory_Backend_NET.Fitur.Pengajuan.PostPengajuan._ResultObject;

public class OnlyAdminCanInputPengajuanStatus : ResultObject
{
    public string Message => "Hanya admin yang dapat menginputkan status pengajuan";
    public string Type => "OnlyAdminCanEditPengajuanStatus";
    public HttpStatusCode StatusCode => HttpStatusCode.Unauthorized;
}