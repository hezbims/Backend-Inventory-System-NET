namespace Inventory_Backend_NET.Fitur.Pengajuan.Domain.Exception.Group;

internal sealed class GroupIdNotFound : IBaseTransactionDomainError
{
    public required int Id { get; init; }
}