using System.Net;
using Inventory_Backend_NET.Fitur._Model;

namespace Inventory_Backend_NET.Fitur.Pengajuan.PostPengajuan._ResultObject;

public class AdminMustHaveStatusPengajuan : ResultObject
{
    public string Message => "Admin harus men-submit data status pengajuan yang diinginkan";
    public string Type => "AdminMustHaveStatusPengajuan";
    public HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
}