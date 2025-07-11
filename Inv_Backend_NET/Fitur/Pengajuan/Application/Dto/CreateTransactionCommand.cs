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
    int Quantity,
    string Notes)
{
    internal CreateTransactionItemDto ToDomainDto()
    {
        return new CreateTransactionItemDto(
            ProductId: ProductId,
            Quantity: Quantity,
            Notes: Notes);
    }
}