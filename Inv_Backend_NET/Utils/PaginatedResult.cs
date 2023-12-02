using Inventory_Backend_NET.Constants;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Backend_NET.Utils;

public static class PaginatedResult
{
    public static OkObjectResult Paginate<T>(this ControllerBase c , List<T> data) where T : class
    {
        int totalData = data.Count();
        bool? next = null;
        if (totalData > MyConstants.PageSize)
        {
            data.RemoveAt(totalData - 1);
            next = true;
        }

        return c.Ok(new
        {
            data,
            links = new
            {
                next
            }
        });
    }
}