namespace Inventory_Backend_NET.Fitur.Pengajuan.Domain.ValueObject;

public enum TransactionStatus
{
    Waiting = 0, 
    Prepared = 1, 
    Canceled = 2,
    Confirmed = 3,
    Rejected = 4
}