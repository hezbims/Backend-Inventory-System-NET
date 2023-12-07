using System.Text.Json.Serialization;
using Inventory_Backend_NET.Models;
using Inventory_Backend_NET.Service;
using Inventory_Backend_NET.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using NeoSmart.Caching.Sqlite;

namespace Inventory_Backend_NET.Controllers.Pengajuan;

[Route("api/pengajuan/add")]
public class SubmitPengajuanController : ControllerBase
{
    private readonly MyDbContext _db;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly SqliteCache _cache;
    
    public SubmitPengajuanController(
        MyDbContext db,
        IHttpContextAccessor httpContextAccessor,
        SqliteCache cache
    )
    {
        _db = db;
        _httpContextAccessor = httpContextAccessor;
        _cache = cache;
    }
    
    [HttpPost]
    public IActionResult SubmitPengajuan(
        [FromBody] SubmitPengajuanBody requestBody    
    )
    {
        using var transaction = _db.Database.BeginTransaction();
        try
        {
            var submitter = _db.GetCurrentUserFrom(_httpContextAccessor);
            
            var previousPengajuan = GetPreviousPengajuan(requestBody);
            if (previousPengajuan != null)
            {
                UndoStockUpdateFromPreviousPengajuan(previousPengajuan);
            }

            
            var currentPengajuan = GetCurrentPengajuan(previousPengajuan , requestBody, submitter);
            UpdateStockBarangByCurrentPengajuan(currentPengajuan);

            _db.SaveChanges();
            
            CheckStockValidityAfterStockUpdate(
                previousPengajuan : previousPengajuan,
                currentPengajuan : currentPengajuan
            );
            
            
            var tipeEvent = GetSubmitPengajuanEvent(previousPengajuan , submitter);
            if (tipeEvent != null)
            {
                if (tipeEvent == PengajuanEvent.UserNotifAdmin)
                {
                    foreach (var admin in _db.Users.Where(user => user.IsAdmin))
                    {
                        _cache.SetString(admin.Username , "1");
                    }
                }
                else
                {
                    _cache.SetString(currentPengajuan.User.Username , "1");
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
            transaction.Rollback();
            return BadRequest(new
            {
                message = e.Message
            });
        }
        catch (Exception e)
        {
            transaction.Rollback();
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

    private Models.Pengajuan GetCurrentPengajuan(
        Models.Pengajuan? previousPengajuan,
        SubmitPengajuanBody requestBody,
        User submitter
    )
    {
        var currentPengaju = _db.Pengajus.Single(pengaju => pengaju.Id == requestBody.IdPengaju);

        
        var currentBarangAjuans = requestBody.BarangAjuans.Select(
            barangAjuanBody => barangAjuanBody.ToBarangAjuan()
        ).ToList();

        if (currentBarangAjuans.Count == 0)
        {
            throw new BadHttpRequestException("Tolong ajukan minimal satu barang!");
        }

        
        User pemilikPengajuan = previousPengajuan?.User ?? submitter;

        
        StatusPengajuan statusPengajuan;

        int totalQuantityOfCurrentPengajuan = requestBody.BarangAjuans.Sum(barangAjuan => barangAjuan.Quantity);
        if (previousPengajuan == null && totalQuantityOfCurrentPengajuan == 0)
        {
            throw new BadHttpRequestException("Tidak boleh mengajukan dengan total quantity 0");
        }
        if (previousPengajuan != null && totalQuantityOfCurrentPengajuan == 0)
        {
            statusPengajuan = StatusPengajuan.Ditolak;
        }
        else
        {
            statusPengajuan = StatusPengajuan.GetByEditor(submitter);
        }

        Models.Pengajuan currentPengajuan = new Models.Pengajuan(
            cache: _cache,
            pengaju: currentPengaju,
            status: statusPengajuan,
            user: pemilikPengajuan,
            barangAjuans: currentBarangAjuans,
            id: previousPengajuan?.Id
        );
            
        if (previousPengajuan == null)
        {
            _db.Pengajuans.Add(currentPengajuan);
        }
        else
        {
            _db.BarangAjuans.RemoveRange(previousPengajuan.BarangAjuans);
            _db.Pengajuans.Update(currentPengajuan);
        }

        return currentPengajuan;
    }

    private PengajuanEvent? GetSubmitPengajuanEvent(
        Models.Pengajuan? previousPengajuan,
        User submitter        
    )
    {
        // Kalo sekarang ini lagi ngebuat pengajuan baru
        if (previousPengajuan == null)
        {
            if (!submitter.IsAdmin)
            {
                return PengajuanEvent.UserNotifAdmin;
            }
        }
        // Kalo lagi ngedit pengajuan sebelumnya
        else
        {
            // Kalo yang mensubmit sekarang bukan admin
            if (!submitter.IsAdmin) 
            {
                // Non-Admin enggak bisa ngedit pengajuan yang bukan miliknya
                if (previousPengajuan.User.Id != submitter.Id)
                {
                    throw new BadHttpRequestException(
                        "Pengajuan ini bukan milik anda"
                    );
                }

                // Non-Admin enggak bisa ngedit pengajuan yang udah ditolak atau diterima
                if (previousPengajuan.Status.Value != StatusPengajuan.MenungguValue)
                {
                    throw new BadHttpRequestException(
                        "Pengajuan yang sudah dikonfirmasi tidak dapat diedit"
                    );
                }

                return PengajuanEvent.UserNotifAdmin;
            }
            else if (previousPengajuan.Status.Value == StatusPengajuan.MenungguValue)
            {
                return PengajuanEvent.AdminNotifUser;
            }
        }

        return null;
    }

    private void CheckStockValidityAfterStockUpdate(
        Models.Pengajuan? previousPengajuan,
        Models.Pengajuan currentPengajuan
    )
    {
        var previousModifiedBarangId =
            previousPengajuan?.BarangAjuans.Select(barangAjuan => barangAjuan.Id).ToList() ?? new List<int>();
        var currentModifiedBarangId =
            currentPengajuan.BarangAjuans.Select(barangAjuan => barangAjuan.Id).ToList();
            
        var allModifiedBarangId = new List<int>();
        allModifiedBarangId.AddRange(previousModifiedBarangId);
        allModifiedBarangId.AddRange(currentModifiedBarangId);

        var modifiedBarangs = _db.Barangs.Where(barang => allModifiedBarangId.Contains(barang.Id));
        var invalidBarang = modifiedBarangs.FirstOrDefault(barang => barang.CurrentStock < 0);
        if (invalidBarang != null)
        {
            throw new BadHttpRequestException(
                message: "Pengajuan tidak valid :\n" +
                         $"quantity dari barang {invalidBarang.Nama}," +
                         $"menjadi {invalidBarang.CurrentStock}"
            );
        }
    }

    private void UpdateStockBarangByCurrentPengajuan(
        Models.Pengajuan currentPengajuan    
    )
    {
        _db.UpdateStockBarang(
            pengajuan: currentPengajuan,
            isUndo: false
        );
    }
    
    private void UndoStockUpdateFromPreviousPengajuan(
        Models.Pengajuan previousPengajuan    
    )
    {
        _db.UpdateStockBarang(
            pengajuan: previousPengajuan,
            isUndo: true
        );
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

enum PengajuanEvent
{
    AdminNotifUser , UserNotifAdmin
}

