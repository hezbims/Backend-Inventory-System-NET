using Inventory_Backend_NET.Common.Presentation.Validation;
using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Database.Models;
using Inventory_Backend_NET.Fitur._Logic.Extension;
using Inventory_Backend_NET.Fitur.Pengajuan.Application.Handler;
using Inventory_Backend_NET.Fitur.Pengajuan.Presentation.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Backend_NET.Fitur.Pengajuan.Presentation;

[Route("api/[controller]")]
internal sealed class TransactionController(
    CreateTransactionHandler createTransactionHandler,
    IHttpContextAccessor httpContextAccessor,
    MyDbContext dbContext) : ControllerBase
{
    [HttpPost]
    internal async Task<IActionResult> PostTransaction(
        [FromBody] PostTransactionRequestBody request,
        CancellationToken cancellationToken)
    {
        User? user = await dbContext.GetCurrentUserFromAsync(httpContextAccessor , cancellationToken);
        if (user == null)
            return Unauthorized();
        
        request.SetUserCreator(isAdmin: user.IsAdmin, id: user.Id);
        throw new NotImplementedException();
        // if (!request.TryValidate(out var validationResults))
        //     return this.ValidationProblem(validationResults);
        
        // var errors = await createTransactionHandler.Handle(
        //     request.ToApplicationDto(), cancellationToken);
        // if (errors.Any())
        //     return this.ValidationProblem(errors);

        return Ok();
    }
}