using Inventory_Backend_NET.Fitur.Pengajuan.Domain.ValueObject;

namespace Inventory_Backend_NET.Fitur.Pengajuan.Domain.Exception.ConfirmTransaction;

public record NonAdminCanOnlyConfirmPreparedTransaction(
    TransactionStatus CurrentStatus) : IBaseTransactionDomainError;