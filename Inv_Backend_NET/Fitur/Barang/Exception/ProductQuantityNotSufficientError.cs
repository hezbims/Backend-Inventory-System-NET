namespace Inventory_Backend_NET.Fitur.Barang.Exception;

/// <summary>
/// Kalau user berusaha mengambil barang lebih banyak dari current quantity
/// </summary>
internal sealed class ProductQuantityNotSufficientError : BaseProductDomainError
{
    public required int CurrentQuantity { get; init; }
    public required int TakenQuantity { get; init; }
    public required int ProductId { get; init; }
}