using System.Net;
using Inventory_Backend_NET.Fitur._Model;

namespace Inventory_Backend_NET.Fitur.Pengajuan.PostPengajuan._ResultObject;

public class NonAdminCanNotEditAcceptedOrRejectedPengajuan : ResultObject
{
    public string Message => "User biasa tidak dapat mengedit pengajuan yang sudah diterima atau ditolak";
    public string Type => "NonAdminCanNotEditAcceptedOrRejectedPengajuan";
    public HttpStatusCode StatusCode => HttpStatusCode.Forbidden;
}