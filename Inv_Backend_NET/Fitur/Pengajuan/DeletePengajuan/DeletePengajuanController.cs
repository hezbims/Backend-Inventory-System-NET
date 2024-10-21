using System.Data;
using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Database.Models;
using Inventory_Backend_NET.Fitur._Constants;
using Inventory_Backend_NET.Fitur._Logic.Extension;
using Inventory_Backend_NET.Fitur._Model;
using Inventory_Backend_NET.Fitur.Pengajuan._Model.Exception;
using Inventory_Backend_NET.UseCases.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Backend_NET.Fitur.Pengajuan.DeletePengajuan;

[Route("api/pengajuan/delete")]
[Authorize(MyConstants.Policies.AllUsers)]
public class DeletePengajuanController : ControllerBase
{
    private readonly MyDbContext _db;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UpdatePengajuanUseCase _updateStock;
    
    public DeletePengajuanController(
        MyDbContext db,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _db = db;
        _httpContextAccessor = httpContextAccessor;
        _updateStock = new UpdatePengajuanUseCase(db);
    }

    [HttpDelete]
    [Route("{idPengajuan}")]
    public IActionResult Index(int? idPengajuan)
    {
        using var transaction = _db.Database.BeginTransaction(IsolationLevel.Serializable);
        try
        {
            var currentPengajuan = _db.Pengajuans
                .Include(pengajuan => pengajuan.User)
                .Include(pengajuan => pengajuan.Pengaju)
                .Include(pengajuan => pengajuan.BarangAjuans)
                .AsSplitQuery()
                .FirstOrDefault(pengajuan => pengajuan.Id == idPengajuan);

            if (currentPengajuan == null)
                return new PengajuanNotFound(pengajuanId: idPengajuan).ToHttpJsonResponse(this);

            var currentUser = _db.GetCurrentUserFrom(_httpContextAccessor);
            var validationResult = AuthorizeDeletePengajuan(
                currentPengajuan, currentUser!
            );
            if (validationResult != null)
                return validationResult.ToHttpJsonResponse(this);


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

    private ResultObject? AuthorizeDeletePengajuan(
        Database.Models.Pengajuan currentPengajuan,
        User currentUser
    )
    {
        // Admin bisa ngehapus pengajuan sesuka hati
        if (currentUser.IsAdmin)
            return null;

        // User mencoba menghapus pengajuan yang bukan miliknya,
        // hanya admin yang dapat menghapus semua jenis pengajuan
        if (currentPengajuan.User.Id != currentUser.Id)
            return new NotOwnedDeletedPengajuan();

        // User boleh ngehapus pengajuan yang statusnya masih menunggu
        if (currentPengajuan.Status.Value == StatusPengajuan.MenungguValue)
            return null;

        return new PengajuanWithUndeletableStatus();
    }
}