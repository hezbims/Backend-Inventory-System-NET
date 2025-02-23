using System.Text.Json.Serialization;

namespace Inventory_Backend_NET.Fitur._Logic.Extension;

public static class PaginatorExtension
{
    public static PaginatedResult<T> Paginate<T>(
        this IQueryable<T> query,
        int pageNumber,
        int pageSize = 50
    )
    {
        int total = query.Count();
        
        var data = query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize + 1)
            .ToList();
        
        
        int totalData = data.Count;
        int? prevPage = pageNumber == 1 ? null : pageNumber - 1;
        int? nextPage = null;
        if (totalData > pageSize)
        {
            data.RemoveAt(totalData - 1);
            nextPage = pageNumber + 1;
        }

        return new PaginatedResult<T>
        {
            Data = data,
            Links = new PaginatedResult<T>.LinkInfo
            {
                CurrentPage = pageNumber,
                NextPage = nextPage,
                PrevPage = prevPage,
                TotalPage = total / pageSize + (total % pageSize == 0 ? 0 : 1),
            }
        };
    }
}

public class PaginatedResult<T>
{
    [JsonPropertyName("data")] 
    public required List<T> Data { get; init; }

    [JsonPropertyName("links")] 
    public required LinkInfo Links{ get; init; }

    public class LinkInfo
    {
        [JsonPropertyName("next")]
        public bool HasNext => NextPage != null;

        [JsonPropertyName("current_page")]
        public required int CurrentPage { get; init; }
        [JsonPropertyName("next_page")]
        public required int? NextPage { get; init; }
        [JsonPropertyName("prev_page")]
        public required int? PrevPage { get; init; }
        [JsonPropertyName("total_page")]
        public required int TotalPage { get; init; }
    }
    
    public PaginatedResult<TO> MapTo<TO>(
        Func<T , TO> mapper    
    )
    {
        return new PaginatedResult<TO>
        {
            Data = Data.Select(mapper).ToList(),
            Links = new PaginatedResult<TO>.LinkInfo
            {
                CurrentPage = Links.CurrentPage,
                NextPage = Links.NextPage,
                PrevPage = Links.PrevPage,
                TotalPage = Links.TotalPage
            }
        };
    }
}