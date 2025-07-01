using System.Data;
using System.Text.Json.Serialization;
using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Database.Models;
using Inventory_Backend_NET.Fitur._Constants;
using Inventory_Backend_NET.Fitur._Logic.Extension;
using Inventory_Backend_NET.Fitur._Model;
using Inventory_Backend_NET.Fitur.Logging;
using Inventory_Backend_NET.Fitur.Pengajuan._Logic;
using Inventory_Backend_NET.Fitur.Pengajuan.DeletePengajuan._ResultObject;
using Inventory_Backend_NET.Fitur.Pengajuan.PostPengajuan._Logic;
using Inventory_Backend_NET.Fitur.Pengajuan.PostPengajuan._ResultObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Inventory_Backend_NET.Fitur.Pengajuan.PostPengajuan;

[Route("api/pengajuan/add")]
public class PostPengajuanController : ControllerBase
{
    private readonly MyDbContext _db;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UpsertCurrentPengajuanUseCase _updateOrInsertNewPengajuan;
    private readonly UpdateStockByPengajuanUseCase _updateStock;
    private readonly IMyLogger _logger;
    
    public PostPengajuanController(
        MyDbContext db,
        IHttpContextAccessor httpContextAccessor,
        TimeProvider timeProvider,
        IMyLogger logger
    )
    {
        _logger = logger;
        _db = db;
        _httpContextAccessor = httpContextAccessor;
        
        _updateOrInsertNewPengajuan  = new UpsertCurrentPengajuanUseCase(
            db: db, timeProvider: timeProvider);
        _updateStock = new UpdateStockByPengajuanUseCase(db: db);
    }
    
    [HttpPost]
    public IActionResult Index(
        [FromBody] SubmitPengajuanBody requestBody    
    )
    {
        using (var transaction = _db.Database.BeginTransaction(IsolationLevel.Serializable))
        {
            try
            {
                var submitter = _db.GetCurrentUserFrom(_httpContextAccessor)!;
                
                var previousPengajuan = GetPreviousPengajuan(requestBody);
                var validationResult = ValidateRequestBody(
                    requestBody: requestBody,
                    previousPengajuan: previousPengajuan,
                    user: submitter);
                if (validationResult != null)
                    return validationResult.ToHttpJsonResponse(this);
                
                var currentPengajuan = _updateOrInsertNewPengajuan.By(
                    previousPengajuan, 
                    requestBody, 
                    submitter
                );
                
                _updateStock.By(
                    previousPengajuan: previousPengajuan,
                    currentPengajuan: currentPengajuan
                );

                _db.SaveChanges();
                transaction.Commit();
                return Ok(new
                {
                    message = "Sukses"
                });
            }
            catch (Exception e)
            {
                _logger.WriteLine($"QQQ Post pengajuan error : {e.Message}");
                return StatusCode(
                    500,
                    new
                    {
                        message = e.Message
                    }
                );
            }
        }

    }
    
    private Database.Models.Pengajuan? GetPreviousPengajuan(SubmitPengajuanBody requestBody)
    {
        if (requestBody.IdPengajuan is null)
            return null;
        var previousPengajuan = _db.Pengajuans
            .Include(pengajuan => pengajuan.User)
            .Include(pengajuan => pengajuan.Pengaju)
            .Include(pengajuan => pengajuan.BarangAjuans)
            .AsSplitQuery()
            .FirstOrDefault(
                pengajuan => pengajuan.Id == requestBody.IdPengajuan
            );
        return previousPengajuan;
    }

    private ResultObject? ValidateRequestBody(
        SubmitPengajuanBody requestBody,
        Database.Models.Pengajuan? previousPengajuan,
        User user)
    {
        if (requestBody.BarangAjuans.IsNullOrEmpty())
            return new ChooseAtLeastOneBarangAjuan();
        if (requestBody.IdPengaju == default)
            return new PengajuIdRequired();

        if (user.IsAdmin)
        {
            if (requestBody.StatusPengajuan.IsNullOrEmpty())
                return new AdminMustHaveStatusPengajuan();
            if (!StatusPengajuan.IsValidStatusPengajuanString(requestBody.StatusPengajuan!))
                return new StatusPengajuanValueInvalid();
            if (requestBody.StatusPengajuan == StatusPengajuan.MenungguValue)
                return new AdminCanNotEditPengajuanToWaitingStatus();
        }
        else
        {
            if (requestBody.StatusPengajuan is not null)
                return new OnlyAdminCanInputPengajuanStatus();
            if (previousPengajuan?.Status == StatusPengajuan.Diterima ||
                previousPengajuan?.Status == StatusPengajuan.Ditolak)
                return new NonAdminCanNotEditAcceptedOrRejectedPengajuan();
        }

        if (previousPengajuan?.Status == StatusPengajuan.Diterima ||
            previousPengajuan?.Status == StatusPengajuan.Ditolak)
        {
            if (requestBody.StatusPengajuan is not null &&
                previousPengajuan.Status.Value != requestBody.StatusPengajuan)
                return new DitolakOrDiterimaPengajuanStatusCantBeChanged();
        }

        if (previousPengajuan == null && requestBody.IdPengajuan != null)
            return new PengajuanNotFound(requestBody.IdPengajuan);

        return null;
    }
}





public class SubmitPengajuanBody
{
    [JsonPropertyName("id")]
    public int? IdPengajuan { get; set; }
    
    [JsonPropertyName("id_pengaju")]
    public int IdPengaju { get; set; }
    
    [JsonPropertyName("list_barang_ajuan")]
    public ICollection<BarangAjuanBody>? BarangAjuans { get; set; }
    
    [JsonPropertyName("status_pengajuan")]
    public string? StatusPengajuan { get; set; }
}

public class BarangAjuanBody
{
    [JsonPropertyName("quantity")] 
    public int Quantity { get; set; }
    
    [JsonPropertyName("keterangan")]
    public string? Keterangan { get; set; }
    
    [JsonPropertyName("id_barang")]
    public int IdBarang { get; set; }

    public BarangAjuan ToBarangAjuan()
    {
        return new BarangAjuan(
            barangId: IdBarang,
            quantity: Quantity,
            keterangan: Keterangan 
        );
    }
}

public enum PengajuanEvent
{
    AdminNotifUser , UserNotifAdmin
}

