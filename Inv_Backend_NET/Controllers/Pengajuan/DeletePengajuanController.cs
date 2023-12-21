using System.Data;
using Inventory_Backend_NET.Constants;
using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Extension;
using Inventory_Backend_NET.Models;
using Inventory_Backend_NET.UseCases.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Backend_NET.Controllers.Pengajuan;

[Route("api/pengajuan/delete")]
[Authorize(MyConstants.Policies.AllUsers)]
public class DeletePengajuanController : ControllerBase
{
    private readonly MyDbContext _db;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UpdateStockUseCase _updateStock;
    
    public DeletePengajuanController(
        MyDbContext db,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _db = db;
        _httpContextAccessor = httpContextAccessor;
        _updateStock = new UpdateStockUseCase(db);
    }

    [HttpDelete]
    [Route("{idPengajuan}")]
    public IActionResult Index(int? idPengajuan)
    {
        using var transaction = _db.Database.BeginTransaction(IsolationLevel.Serializable);
        try
        {
            var currentPengajuan = _db.Pengajuans
                .Include(pengajuan => pengajuan.Pengaju)
                .Include(pengajuan => pengajuan.BarangAjuans)
                .FirstOrDefault(pengajuan => pengajuan.Id == idPengajuan);

            if (currentPengajuan == null)
                return Unauthorized(new
                {
                    message = "Pengajuan tidak ditemukan"
                });

            var currentUser = _db.GetCurrentUserFrom(_httpContextAccessor);
            var authorizeResult = AuthorizeDeletePengajuan(
                currentPengajuan , currentUser!
            );
            if (authorizeResult != null)
                return BadRequest(new
                {
                    message = authorizeResult
                });

            
            _updateStock.By(
                previousPengajuan: currentPengajuan,
                currentPengajuan: null
            );
            _db.Pengajuans.Remove(currentPengajuan);
            _db.SaveChanges();
            transaction.Commit();

            return Ok(new
            {
                message = "Sukses"
            });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500 , new
            {
                message = e.Message
            });
        }
    }

    private string? AuthorizeDeletePengajuan(
        Models.Pengajuan currentPengajuan,
        User currentUser
    )
    {
        // Admin bisa ngehapus pengajuan sesuka hati
        if (currentUser.IsAdmin)
            return null;

        // User boleh ngehapus pengajuan yang statusnya masih menunggu
        if (currentPengajuan.Status.Value == StatusPengajuan.MenungguValue)
            return null;

        return $"Pengajuan yang statusnya '{currentPengajuan.Status.Value}' tidak dapat dihapus oleh non-admin";
    }
}