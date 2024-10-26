using System.Net;
using Inventory_Backend_NET.Fitur._Model;

namespace Inventory_Backend_NET.Fitur.Pengajuan.PostPengajuan._ResultObject;

public class DitolakOrDiterimaPengajuanStatusCantBeChanged : ResultObject
{
    public string Message => "Pengajuan dengan status diterima atau ditolak tidak dapat diubah lagi statusnya";
    public string Type => "DitolakOrDiterimaPengajuanStatusCantBeChanged";
    public HttpStatusCode StatusCode => HttpStatusCode.Forbidden;
}