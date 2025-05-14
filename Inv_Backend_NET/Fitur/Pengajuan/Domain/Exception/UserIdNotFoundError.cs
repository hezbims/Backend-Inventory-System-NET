namespace Inventory_Backend_NET.Fitur.Pengajuan.Domain.Exception;

public class UserIdNotFoundError(int Id) : 
    System.Exception($"Transaction - User with id '{Id}' not found"),
    IBaseTransactionDomainError;