using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Entity;

namespace Inventory_Backend_NET.Fitur.Pengajuan.Domain.Repository;

public interface ITransactionRepository
{
    public void Save(Transaction transaction);
}