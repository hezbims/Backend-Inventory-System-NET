using System.Text.Json.Serialization;

namespace Inventory_Backend_NET.Common.Presentation.Model;

internal record MyValidationError(
    [property: JsonPropertyName("code")] String Code, 
    [property: JsonPropertyName("message")] String Message);