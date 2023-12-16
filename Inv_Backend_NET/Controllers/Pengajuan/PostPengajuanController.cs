using System.Data;
using System.Text.Json.Serialization;
using Inventory_Backend_NET.Constants;
using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Extension;
using Inventory_Backend_NET.Models;
using Inventory_Backend_NET.UseCases.Common;
using Inventory_Backend_NET.UseCases.PostPengajuan;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace Inventory_Backend_NET.Controllers.Pengajuan;

[Authorize(MyConstants.Policies.AllUsers)]
[Route("api/pengajuan/add")]
public class PostPengajuanController : ControllerBase
{
    private readonly MyDbContext _db;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IDistributedCache _cache;
    private readonly UpsertCurrentPengajuanUseCase _updateOrInsertNewPengajuan;
    private readonly UpdateStockUseCase _updateStock;
    private readonly GetEventTypeUseCase _getEventType;
    
    public PostPengajuanController(
        MyDbContext db,
        IHttpContextAccessor httpContextAccessor,
        IDistributedCache cache
    )
    {
        _db = db;
        _httpContextAccessor = httpContextAccessor;
        _cache = cache;
        
        _updateOrInsertNewPengajuan  = new UpsertCurrentPengajuanUseCase(db: db, cache: _cache);
        _updateStock = new UpdateStockUseCase(db: db);
        _getEventType = new GetEventTypeUseCase();
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
                var currentPengajuan = _updateOrInsertNewPengajuan.Exec(
                    previousPengajuan, 
                    requestBody, 
                    submitter
                );
                
                _updateStock.By(
                    previousPengajuan: previousPengajuan,
                    currentPengajuan: currentPengajuan
                );

                var tipeEvent = _getEventType.Exec(previousPengajuan, submitter);
                if (tipeEvent != null)
                {
                    if (tipeEvent == PengajuanEvent.UserNotifAdmin)
                    {
                        foreach (var admin in _db.Users.Where(user => user.IsAdmin))
                        {
                            _cache.SetString(admin.Username, "1");
                        }
                    }
                    else
                    {
                        _cache.SetString(currentPengajuan.User.Username, "1");
                    }
                }

                transaction.Commit();
                return Ok(new
                {
                    message = "Sukses"
                });
            }
            catch (BadHttpRequestException e)
            {
                return BadRequest(new
                {
                    message = e.Message
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
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
    
    private Models.Pengajuan? GetPreviousPengajuan(SubmitPengajuanBody requestBody)
    {
        var previousPengajuan = _db.Pengajuans
            .Include(pengajuan => pengajuan.User)
            .Include(pengajuan => pengajuan.Pengaju)
            .Include(pengajuan => pengajuan.BarangAjuans)
            .FirstOrDefault(
                pengajuan => pengajuan.Id == requestBody.IdPengajuan
            );
        return previousPengajuan;
    }
        
}





public class SubmitPengajuanBody
{
    [JsonPropertyName("id")]
    public int? IdPengajuan { get; set; }
    
    [JsonPropertyName("id_pengaju")]
    public int IdPengaju { get; set; }
    
    [JsonPropertyName("list_barang_ajuan")]
    public ICollection<BarangAjuanBody> BarangAjuans { get; set; }
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

