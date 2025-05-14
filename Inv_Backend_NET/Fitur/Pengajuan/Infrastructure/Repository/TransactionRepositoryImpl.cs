using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Entity;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Repository;

namespace Inventory_Backend_NET.Fitur.Pengajuan.Infrastructure.Repository;

public class TransactionRepositoryImpl(MyDbContext dbContext) : ITransactionRepository
{
    public void Save(Transaction transaction)
    {
        throw new NotImplementedException();
    }
}