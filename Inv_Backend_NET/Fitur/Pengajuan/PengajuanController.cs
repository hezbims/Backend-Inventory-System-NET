using Inventory_Backend_NET.Fitur._Constants;
using Inventory_Backend_NET.Fitur.Pengajuan._Cqrs.Query;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Backend_NET.Fitur.Pengajuan;

[Route("api/pengajuan")]
[Authorize(policy : MyConstants.Policies.AllUsers)]
public class PengajuanController(
    GetPengajuanSse getPengajuanSse)
    : Controller
{
    private readonly GetPengajuanSse _getPengajuanSse = getPengajuanSse;

    [HttpGet("event")]
    public async Task GetPengajuanServerSentEvent(CancellationToken cancellationToken)
    {
        await _getPengajuanSse.Execute(cancellationToken: cancellationToken);
    }
}