using Newtonsoft.Json;

namespace Inventory_Backend.Tests.Helper;

public static class JsonParserHelper
{
    public static async Task<T> ToModel<T>(this HttpResponseMessage response)
    {
        return JsonConvert.DeserializeObject<T>(
            await response.Content.ReadAsStringAsync())!;
    }
}