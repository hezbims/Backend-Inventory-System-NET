namespace Inventory_Backend_NET.Fitur.Barang.Exception;

internal sealed class ProductIdNotFoundError : BaseProductDomainError
{
    public required int Id { get; init; }
}