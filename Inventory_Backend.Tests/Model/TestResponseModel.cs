using System.Text.Json;

namespace Inventory_Backend.Tests.Model;

public class TestResponseModel
{
    public required string Message { get; init; }
    public required string Type { get; init; }
    public required int StatusCode { get; init; }

    public static async Task<TestResponseModel> From(HttpResponseMessage response)
    {
        var json = JsonSerializer.Deserialize<Dictionary<string, Object>>(await response.Content.ReadAsStringAsync())!;

        return new TestResponseModel
        {
            Message = json["message"].ToString()!,
            Type = json["type"].ToString()!,
            StatusCode = (int)response.StatusCode
        };
    }
}