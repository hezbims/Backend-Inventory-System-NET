namespace Inventory_Backend.Tests.Fitur.Product.PostProductCsvTest._Dto;

public record PostProductCsvErrorDto(
    Dictionary<string, List<string>> Errors);