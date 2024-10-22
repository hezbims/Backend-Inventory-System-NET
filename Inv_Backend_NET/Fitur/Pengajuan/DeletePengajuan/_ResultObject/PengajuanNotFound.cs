using System.Net;
using Inventory_Backend_NET.Fitur._Model;

namespace Inventory_Backend_NET.Fitur.Pengajuan.DeletePengajuan._ResultObject;

public class PengajuanNotFound : ResultObject
{
    private readonly int? _pengajuanId;
    public PengajuanNotFound(int? pengajuanId)
    {
        _pengajuanId = pengajuanId;
    }

    public string Message => $"Pengajuan dengan id '{_pengajuanId}' tidak ditemukan.";
    public string Type => "PengajuanNotFound";
    public HttpStatusCode StatusCode => HttpStatusCode.NotFound;
}