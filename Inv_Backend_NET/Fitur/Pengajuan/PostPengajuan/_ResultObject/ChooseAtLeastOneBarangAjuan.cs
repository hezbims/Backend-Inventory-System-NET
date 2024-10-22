using System.Net;
using Inventory_Backend_NET.Fitur._Model;

namespace Inventory_Backend_NET.Fitur.Pengajuan.PostPengajuan._ResultObject;

public class ChooseAtLeastOneBarangAjuan : ResultObject
{
    public string Message => "Tolong inputkan setidaknya satu barang ajuan";
    public string Type => "ChooseAtLeastOneBarangAjuan";
    public HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
}