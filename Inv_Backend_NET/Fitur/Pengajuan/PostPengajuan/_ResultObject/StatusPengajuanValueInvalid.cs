using System.Net;
using Inventory_Backend_NET.Fitur._Model;

namespace Inventory_Backend_NET.Fitur.Pengajuan.PostPengajuan._ResultObject;

public class StatusPengajuanValueInvalid : ResultObject
{
    public string Message => "Nilai string dari status pengajuan tidak valid";
    public string Type => "StatusPengajuanInvalid";
    public HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
}