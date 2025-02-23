using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Fitur._Logic.Extension;
using Inventory_Backend_NET.Fitur.Barang._Data;
using Inventory_Backend_NET.Fitur.Barang._Dto.Response;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Backend_NET.Fitur.Barang._Cqrs.Query;

public class GetBarangsQuery
{
    private readonly MyDbContext _db;

    public GetBarangsQuery(MyDbContext db)
    {
        _db = db;
    }

    public PaginatedResult<BarangDto> Execute(GetBarangsRequestParams requestParams)
    {
        
        var query = _db.Barangs.Where(barang =>
                EF.Functions.Like(
                    barang.Nama,
                    $"%{requestParams.SearchKeyword}%"
                ) ||
                EF.Functions.Like(
                    barang.KodeBarang,
                    $"%{requestParams.SearchKeyword}%"
                )
            );
        if (requestParams.IdKategori != null && requestParams.IdKategori > 0)
            query = query.Where(barang => barang.KategoriId == requestParams.IdKategori);
                
        var result = query.Include(barang => barang.Kategori)
            .OrderByDescending(barang => barang.Id)
            .Paginate(pageNumber: requestParams.Page ?? 1)
            .MapTo(barang => BarangDto.From(barang));

        return result;
    }
}