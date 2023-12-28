using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Inventory_Backend_NET.Fitur._Logic.Extension;

public static class ModelStateTransformerExtension
{
    public static Dictionary<string , List<string>> ToMinimalDictionary(
        this ModelStateDictionary modelState    
    )
    {
        return modelState
            .Where(x => x.Value?.Errors.Any() ?? false)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToList()
            );
    }

    public static List<string> ToMinimalList(
        this ModelStateDictionary modelState
    )
    {
        return modelState.ToMinimalDictionary()
            .Select(
                keyValuePair => $"{keyValuePair.Key} : {keyValuePair.Value}" 
            ).ToList();
    }
}