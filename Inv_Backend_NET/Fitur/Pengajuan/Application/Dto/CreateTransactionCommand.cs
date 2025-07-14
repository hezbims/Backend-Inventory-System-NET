using Inventory_Backend_NET.Common.Domain.ValueObject;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.TransactionItem;

namespace Inventory_Backend_NET.Fitur.Pengajuan.Application.Dto;

internal sealed record CreateTransactionCommand(
    TransactionType Type,
    int CreatorId,
    int? AssignedUserId,
    long TransactionTime,
    int GroupId,
    string Notes,
    List<CreateTransactionItemCommand> TransactionItems);

internal sealed record CreateTransactionItemCommand(
    int ProductId,
    int ExpectedQuantity,
    int? PreparedQuantity,
    string Notes)
{
    internal CreateOutTypeTransactionItemDto ToCreateOutTypeDomainDto()
    {
        return new CreateOutTypeTransactionItemDto(
            ProductId: ProductId,
            ExpectedQuantity: ExpectedQuantity,
            PreparedQuantity: PreparedQuantity,
            Notes: Notes);
    }

    internal CreateInTypeTransactionItemDto ToCreateInTypeDomainDto()
    {
        return new CreateInTypeTransactionItemDto(
            ProductId: ProductId,
            Quantity: ExpectedQuantity,
            Notes: Notes);
    }
}