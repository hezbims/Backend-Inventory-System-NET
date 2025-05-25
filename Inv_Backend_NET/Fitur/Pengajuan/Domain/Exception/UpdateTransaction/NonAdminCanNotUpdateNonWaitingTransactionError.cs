using Inventory_Backend_NET.Fitur.Pengajuan.Domain.ValueObject;

namespace Inventory_Backend_NET.Fitur.Pengajuan.Domain.Exception.UpdateTransaction;

public class NonAdminCanNotUpdateNonWaitingTransactionError(
    TransactionStatus currentTransactionStatus) : IBaseTransactionDomainError;