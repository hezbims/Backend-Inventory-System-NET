using Inventory_Backend_NET.Fitur._Constants;
using Inventory_Backend_NET.Fitur.Pengajuan._Cqrs.Query;
using Inventory_Backend_NET.Fitur.Pengajuan._Dto.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Backend_NET.Fitur.Pengajuan;

[Route("api/pengajuan")]
public class PengajuanController(
    GetPengajuanSse getPengajuanSse,
    GetPengajuans getPengajuans)
    : Controller
{

    [HttpGet]
    public async Task<IActionResult> GetPengajuans(
        GetPengajuansRequestParams requestParams,
        CancellationToken cancellationToken)
    {
        return Ok(await getPengajuans.Execute(requestParams, cancellationToken));
    }
    
    [HttpGet("event")]
    public async Task GetPengajuanServerSentEvent(CancellationToken cancellationToken)
    {
        await getPengajuanSse.Execute(cancellationToken: cancellationToken);
    }
}