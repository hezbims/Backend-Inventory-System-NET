using Inventory_Backend_NET.Common.Presentation.Validation;
using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Database.Models;
using Inventory_Backend_NET.Fitur._Logic.Extension;
using Inventory_Backend_NET.Fitur.Pengajuan.Presentation.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Backend_NET.Fitur.Pengajuan.Presentation;

[Route("api/[controller]")]
public class TransactionController(
    IHttpContextAccessor httpContextAccessor,
    MyDbContext dbContext) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> PostTransaction(
        [FromBody] PostTransactionRequestDto request,
        CancellationToken token)
    {
        User? user = await dbContext.GetCurrentUserFromAsync(httpContextAccessor , token);
        if (user == null)
            return Unauthorized();
        
        request.SetUserCreator(user);
        if (!request.TryValidate(out var validationResults))
            return this.ValidationProblem(validationResults);
        
        return Ok();
    }
}