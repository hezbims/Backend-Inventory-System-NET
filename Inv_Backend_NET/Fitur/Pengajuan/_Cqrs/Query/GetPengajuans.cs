using System.Diagnostics.CodeAnalysis;
using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Database.Models;
using Inventory_Backend_NET.Fitur._Constants;
using Inventory_Backend_NET.Fitur._Logic.Extension;
using Inventory_Backend_NET.Fitur.Pengajuan._Dto.Request;
using Inventory_Backend_NET.Fitur.Pengajuan._Dto.Response;
using Inventory_Backend_NET.Fitur.Pengajuan._Logic;
using Inventory_Backend_NET.Fitur.Pengajuan._Mapper;
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
    
    [SuppressMessage("ReSharper", "EntityFramework.ClientSideDbFunctionCall")]
    public async Task<GetPengajuansResponseDto> Execute(
        GetPengajuansRequestParams requestParams,
        CancellationToken cancellationToken)
    {
        User currentUser = (await _dbContext.GetCurrentUserFromAsync(
            _httpContextAccessor,
            cancellationToken: cancellationToken))!;
        
        var query = _dbContext.Pengajuans
            .Join(_dbContext.Pengajus,
                pengajuan => pengajuan.PengajuId,
                pengaju => pengaju.Id,
                (pengajuan, pengaju) => new {pengajuan, pengaju})
            .Join(_dbContext.Users,
                prevJoin => prevJoin.pengajuan.UserId,
                user => user.Id,
                (prevJoin, user) => new {prevJoin.pengajuan, prevJoin.pengaju , user})
            .Where(data => 
                data.pengajuan.WaktuUpdate < requestParams.LastDate ||
                data.pengajuan.WaktuUpdate == requestParams.LastDate &&
                data.pengajuan.Id < requestParams.LastId);
        
        if (!requestParams.SearchKeyword.IsNullOrEmpty())
        {
            string escapedSearchKeyword = requestParams.SearchKeyword
                .Replace("\\", "\\\\") 
                .Replace("%", "\\%")
                .Replace("_", "\\_")
                .Replace("[", "\\["); 
            query = query.Where(data =>
                EF.Functions.Like(data.pengajuan.KodeTransaksi, $"%{escapedSearchKeyword}%") ||
                EF.Functions.Like(data.pengaju.Nama , $"%{escapedSearchKeyword}%"));
        }
        if (requestParams.IdPengaju != null)
        {
            query = query.Where(data =>
                data.pengajuan.PengajuId == requestParams.IdPengaju);
        }
        
        if (!currentUser.IsAdmin)
        {
            query = query.Where(data =>
                data.pengajuan.UserId == currentUser.Id);
        }
        
        query = query
            .OrderByDescending(data => data.pengajuan.WaktuUpdate)
            .ThenByDescending(data => data.pengajuan.Id);
        
        List<GetPengajuansResponseDto.ResultData> data = query
            .Take(50)
            .AsNoTracking()
            .ToList()
            .Select(data =>
            {
                data.pengajuan.Pengaju = data.pengaju;
                data.pengajuan.User = data.user;
                return data.pengajuan.ToGetPengajuansResponseDtoData();
            })
            .ToList();
        
        bool hasNextPage = data.Count == MyConstants.DefaultPageSize + 1;
        if (hasNextPage)
            data.RemoveAt(MyConstants.DefaultPageSize);

        int version = memoryCache.GetListTransactionVersion(currentUser);

        return new GetPengajuansResponseDto
        {
            Meta = new GetPengajuansResponseDto.MetaData(version, hasNextPage),
            Data = data
        };
    }
}