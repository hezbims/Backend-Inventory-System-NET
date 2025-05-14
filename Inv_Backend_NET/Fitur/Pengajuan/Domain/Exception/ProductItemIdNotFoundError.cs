namespace Inventory_Backend_NET.Fitur.Pengajuan.Domain.Exception;

public class ProductItemIdNotFoundError(int Id) : 
    System.Exception($"Transaction - Product Item with id '{Id}' not found"),
    IBaseTransactionDomainError;