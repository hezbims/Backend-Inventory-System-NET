using System.Net;
using Inventory_Backend_NET.Fitur._Model;

namespace Inventory_Backend_NET.Fitur.Pengajuan.PostPengajuan._ResultObject;

public class PengajuIdRequired : ResultObject
{
    public string Message => "Tolong inputkan ID dari pengaju";
    public string Type => "PengajuRequired";
    public HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
}