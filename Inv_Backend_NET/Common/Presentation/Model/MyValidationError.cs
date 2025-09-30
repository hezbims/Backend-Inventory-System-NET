using System.Text.Json.Serialization;

namespace Inventory_Backend_NET.Common.Presentation.Model;

internal record MyValidationError(
    [property: JsonPropertyName("code")] 
    string Code,
    
    [property: JsonPropertyName("message")] 
    string Message,
    
    [property: JsonPropertyName("location")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    string? Location = null);