using Inventory_Backend_NET.Common.Domain.ValueObject;

namespace Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.TransactionItem;

internal sealed record CreateOutTypeTransactionItemDto(
    int ProductId, 
    int ExpectedQuantity, 
    int? PreparedQuantity, 
    string Notes);