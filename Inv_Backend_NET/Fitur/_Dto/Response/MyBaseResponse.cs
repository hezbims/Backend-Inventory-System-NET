using System.Text.Json.Serialization;

namespace Inventory_Backend_NET.Fitur._Dto.Response;

public class MyBaseResponse<T>
{
    [JsonPropertyName("data")]
    public T? Data { get; set; }
}