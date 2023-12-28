using System.Text.Json.Serialization;
using Inventory_Backend_NET.Fitur._Constants;

namespace Inventory_Backend_NET.Fitur._Logic.Extension;

public static class PaginatorExtension
{
    public static PaginatedResult<T> Paginate<T>(
        this IQueryable<T> query,
        int pageNumber
    )
    {
        var data = query
            .Skip((pageNumber - 1) * MyConstants.PageSize)
            .Take(MyConstants.PageSize + 1)
            .ToList();
        
        
        int totalData = data.Count;
        bool? next = null;
        if (totalData > MyConstants.PageSize)
        {
            data.RemoveAt(totalData - 1);
            next = true;
        }

        return new PaginatedResult<T>(
            data: data,
            hasNext: next
        );
    }
}

public class PaginatedResult<T>
{
    [JsonPropertyName("data")] 
    public List<T> Data { get; set; }

    [JsonPropertyName("links")] 
    public LinkInfo Links{ get; init; }

    public class LinkInfo
    {
        [JsonPropertyName("next")] 
        public bool? HasNext { get; init; }

        public LinkInfo(bool? hasNext)
        {
            HasNext = hasNext;
        }
    }
    
    public PaginatedResult(
        List<T> data,
        bool? hasNext
    )
    {
        Data = data;
        Links = new LinkInfo(hasNext: hasNext);
    }
    
    public PaginatedResult<TO> MapTo<TO>(
        Func<T , TO> mapper    
    )
    {
        return new PaginatedResult<TO>(
            data : Data.Select(mapper).ToList(),
            hasNext: Links.HasNext
        );
    }
}