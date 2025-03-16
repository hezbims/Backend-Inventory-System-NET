using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Database.Models;
using Inventory_Backend_NET.Fitur._Constants;
using Inventory_Backend_NET.Fitur._Logic.Extension;
using Microsoft.Extensions.Caching.Memory;

namespace Inventory_Backend_NET.Fitur.Pengajuan._Cqrs.Query;

public class GetPengajuanSse(
    IHttpContextAccessor httpContextAccessor,
    MyDbContext dbContext,
    IMemoryCache memoryCache)
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly MyDbContext _dbContext = dbContext;
    
    public async Task Execute(CancellationToken cancellationToken)
    {
        HttpResponse response = _httpContextAccessor.HttpContext?.Response ??
            throw new InvalidOperationException("HttpContext was null");
        User currentUser = await _dbContext.GetCurrentUserFromAsync(_httpContextAccessor, cancellationToken) ??
            throw new UnauthorizedAccessException();
        
        response.ContentType = "text/event-stream";
        response.Headers.CacheControl = "no-cache";
        response.Headers.Connection = "keep-alive";
        
        while (!cancellationToken.IsCancellationRequested)
        {
            int message = memoryCache.Get<int>(
                currentUser.IsAdmin ? 
                    MyConstants.CacheKeys.PengajuanTableVersionForAdmin :
                    MyConstants.CacheKeys.PengajuanTableVersionByUser(currentUser));
            await response.WriteAsync($"event:get-user-transaction-version\n\n", cancellationToken: cancellationToken);
            await response.WriteAsync($"data:{message}\n\n", cancellationToken: cancellationToken);
            await response.Body.FlushAsync(cancellationToken: cancellationToken);
            
            await Task.Delay(5000, cancellationToken);
        }
    }
}