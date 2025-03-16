using System.Text;
using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Database.Models;
using Inventory_Backend_NET.Fitur._Constants;
using Inventory_Backend_NET.Fitur._Logic.Extension;
using Inventory_Backend_NET.Fitur.Pengajuan._Dto.Request;
using Inventory_Backend_NET.Fitur.Pengajuan._Dto.Response;
using Inventory_Backend_NET.Fitur.Pengajuan._Logic;
using Inventory_Backend_NET.Fitur.Pengajuan._Mapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;

namespace Inventory_Backend_NET.Fitur.Pengajuan._Cqrs.Query;

public class GetPengajuans(
    MyDbContext dbContext,
    IHttpContextAccessor httpContextAccessor,
    IMemoryCache memoryCache)
{
    private readonly MyDbContext _dbContext = dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    
    public async Task<GetPengajuansResponseDto> Execute(
        GetPengajuansRequestParams requestParams,
        CancellationToken cancellationToken)
    {
        User user = (await _dbContext.GetCurrentUserFromAsync(
            _httpContextAccessor,
            cancellationToken: cancellationToken))!;
        
        StringBuilder baseQueryBuilder = new StringBuilder();
        List<object> sqlParams = [];
        
        baseQueryBuilder.Append("SELECT * FROM Pengajuans ");
        baseQueryBuilder.Append("WHERE (WaktuUpdate < @WaktuUpdate OR ");
        baseQueryBuilder.Append("WaktuUpdate = @WaktuUpdate AND Id < @Id) ");
        sqlParams.Add(new SqlParameter("@WaktuUpdate", requestParams.LastDate));
        sqlParams.Add(new SqlParameter("@Id", requestParams.LastId));

        IQueryable<Database.Models.Pengajuan> query = _dbContext.Pengajuans
            .FromSqlRaw(
                baseQueryBuilder.ToString(),
                sqlParams.ToArray()
            );
        
        if (!requestParams.SearchKeyword.IsNullOrEmpty())
        {
            query = query.Where(pengajuan => 
                EF.Functions.Like(
                    pengajuan.KodeTransaksi, 
                    $"%{requestParams.SearchKeyword}%"));
        }
        
        if (requestParams.IdPengaju != null)
        {
            query = query.Where(
                pengaju => pengaju.PengajuId == requestParams.IdPengaju);
        }
        
        if (!user.IsAdmin)
        {
            query = query.Where(
                pengajuan => pengajuan.UserId == user.Id);
        }

        List<GetPengajuansResponseDto.ResultData> data = query
            .Include(pengajuan => pengajuan.User)
            .Include(pengajuan => pengajuan.Pengaju)
            .OrderByDescending(pengajuan => pengajuan.WaktuUpdate)
            .ThenByDescending(pengajuan => pengajuan.Id)
            .Take(MyConstants.DefaultPageSize + 1)
            .ToList()
            .Select(pengajuan => pengajuan.ToGetPengajuansResponseDtoData())
            .ToList();
        
        bool hasNextPage = data.Count == MyConstants.DefaultPageSize + 1;
        if (hasNextPage)
            data.RemoveAt(MyConstants.DefaultPageSize);

        int version = memoryCache.GetListTransactionVersion(user);

        return new GetPengajuansResponseDto
        {
            Meta = new GetPengajuansResponseDto.MetaData(version, hasNextPage),
            Data = data
        };
    }
}