using Inventory_Backend_NET.Constants;

namespace Inventory_Backend_NET.Utils;

public static class PaginatorExtension
{
    public static object Paginate<T>(
        this IQueryable<T> query,
        int pageNumber,
        Func<T , object>? dataMapper = null
    )
    {
        var data = query
            .Skip((pageNumber - 1) * MyConstants.PageSize)
            .Take(MyConstants.PageSize + 1)
            .ToList();

        object result;
        if (dataMapper != null)
        {
            result = data.Select(dataMapper).ToList();
        }
        else
        {
            result = data;
        }
        
        int totalData = data.Count();
        bool? next = null;
        if (totalData > MyConstants.PageSize)
        {
            data.RemoveAt(totalData - 1);
            next = true;
        }

        return new
        {
            result,
            links = new
            {
                next
            }
        };
    }
}