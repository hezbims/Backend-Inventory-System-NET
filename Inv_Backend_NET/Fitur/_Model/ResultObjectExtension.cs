using Microsoft.AspNetCore.Mvc;

namespace Inventory_Backend_NET.Fitur._Model;

public static class ResultObjectExtension
{
    public static IActionResult ToHttpJsonResponse(this ResultObject resultObject, ControllerBase controller)
    {
        return controller.StatusCode(
            (int)resultObject.StatusCode, new
            {
                message = resultObject.Message,
                type = resultObject.Type,
            });
    }
}