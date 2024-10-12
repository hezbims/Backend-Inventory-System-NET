using System.Data;
using System.Text.Json.Serialization;
using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Database.Models;
using Inventory_Backend_NET.Fitur._Constants;
using Inventory_Backend_NET.Fitur._Logic.Extension;
using Inventory_Backend_NET.Fitur.Pengajuan.PostPengajuan._Logic;
using Inventory_Backend_NET.UseCases.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;

namespace Inventory_Backend_NET.Fitur.Pengajuan.PostPengajuan;

[Authorize(MyConstants.Policies.AllUsers)]
[Route("api/pengajuan/add")]
public class PostPengajuanController : ControllerBase
{
    private readonly MyDbContext _db;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IDistributedCache _cache;
    private readonly UpsertCurrentPengajuanUseCase _updateOrInsertNewPengajuan;
    private readonly UpdateStockUseCase _updateStock;
    private readonly GetPengajuanEventTypeUseCase _getPengajuanEventType;
    private readonly TimeProvider _timeProvider;
    
    public PostPengajuanController(
        MyDbContext db,
        IHttpContextAccessor httpContextAccessor,
        IDistributedCache cache,
        TimeProvider timeProvider,
        UpsertCurrentPengajuanUseCase updateOrInsertNewPengajuan
    )
    {
        _db = db;
        _httpContextAccessor = httpContextAccessor;
        _cache = cache;
        
        _updateOrInsertNewPengajuan  = updateOrInsertNewPengajuan;
        _updateStock = new UpdateStockUseCase(db: db);
        _getPengajuanEventType = new GetPengajuanEventTypeUseCase();
        _timeProvider = timeProvider;
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
                if (requestBody.BarangAjuans.IsNullOrEmpty())
                    throw new BadHttpRequestException("Tolong ajukan minimal satu barang");
                if (requestBody.IdPengaju == default)
                    throw new BadHttpRequestException("Tolong pilih pengaju");
                
                var submitter = _db.GetCurrentUserFrom(_httpContextAccessor)!;

                var previousPengajuan = GetPreviousPengajuan(requestBody);
                var currentPengajuan = _updateOrInsertNewPengajuan.By(
                    previousPengajuan, 
                    requestBody, 
                    submitter
                );
                
                _updateStock.By(
                    previousPengajuan: previousPengajuan,
                    currentPengajuan: currentPengajuan
                );

                var tipeEvent = _getPengajuanEventType.Exec(previousPengajuan, submitter);
                if (tipeEvent != null)
                {
                    if (tipeEvent == PengajuanEvent.UserNotifAdmin)
                        foreach (var admin in _db.Users.Where(user => user.IsAdmin))
                            _cache.SetString(admin.Username, "1");
                    
                    else
                        _cache.SetString(currentPengajuan.User.Username, "1");
                }

                if (requestBody.IdPengajuan == null)
                {
                    var currentTanggal = _timeProvider.GetLocalNow().ToString("yyyy-MM-dd");

                    var totalPengajuanByTanggal = _db
                        .TotalPengajuanByTanggals
                        .FirstOrDefault(x => x.Tanggal == currentTanggal);

                    if (totalPengajuanByTanggal == null)
                    {
                        totalPengajuanByTanggal = new TotalPengajuanByTanggal
                        {
                            Tanggal = currentTanggal,
                            Total = 1
                        };
                        _db.Add(totalPengajuanByTanggal);
                    }
                    else
                    {
                        totalPengajuanByTanggal.Total += 1;
                        _db.Update(totalPengajuanByTanggal);
                    }

                    _db.SaveChanges();
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
                Console.WriteLine(e);
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
    public ICollection<BarangAjuanBody>? BarangAjuans { get; set; }
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

