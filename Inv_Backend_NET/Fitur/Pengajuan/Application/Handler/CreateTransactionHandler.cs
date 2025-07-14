using System.Data;
using Inventory_Backend_NET.Common.Domain.Exception;
using Inventory_Backend_NET.Common.Domain.ValueObject;
using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Database.Models;
using Inventory_Backend_NET.Fitur.Autentikasi.Domain.Exception;
using Inventory_Backend_NET.Fitur.Barang.Exception;
using Inventory_Backend_NET.Fitur.Barang.Handler;
using Inventory_Backend_NET.Fitur.Pengajuan.Application.Dto;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.Transaction.Create;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.User;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Entity;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Exception.Group;
using Inventory_Backend_NET.Fitur.Pengajuan.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using ILogger = Serilog.ILogger;

namespace Inventory_Backend_NET.Fitur.Pengajuan.Application.Handler;

using Pengaju = Database.Models.Pengaju;
using Barang = Database.Models.Barang;

internal sealed class CreateTransactionHandler(
    MyDbContext dbContext,
    ILogger logger,
    TransactionRepositoryImpl transactionRepository,
    ProductQuantityChangedEventHandler productQuantityChangedEventHandler)
{
    internal async Task<List<IBaseDomainError>> Handle(
        CreateTransactionCommand command,
        CancellationToken cancellationToken)
    {
        List<IBaseDomainError> errors = new List<IBaseDomainError>();

        await using var dbTransaction = await dbContext.Database.BeginTransactionAsync(
            IsolationLevel.RepeatableRead, cancellationToken);

        try
        {
            ValueTask<User?> creatorTask = dbContext.Users
                .FindAsync([command.CreatorId], cancellationToken);

            ValueTask<Pengaju?> groupTask = dbContext.Pengajus
                .FindAsync([command.GroupId], cancellationToken);

            ValueTask<User?> assignedUserTask = command.AssignedUserId == null
                ? ValueTask.FromResult<User?>(null)
                : dbContext.Users.FindAsync([command.AssignedUserId], cancellationToken);

            List<int> productIds = command.TransactionItems.Select(item => item.ProductId).ToList();
            Task<List<Barang>> productsTask = dbContext.Barangs
                .Where(barang => productIds.Contains(barang.Id))
                .ToListAsync(cancellationToken);
            
            User? creator = await creatorTask;
            Pengaju? group = await groupTask;
            User? assignedUser = await assignedUserTask;
            List<Barang> foundProducts = await productsTask;
            
            if (creator is null)
                errors.Add(new UserIdNotFound { Id = command.CreatorId });
            if (group is null)
                errors.Add(new GroupIdNotFound { Id = command.GroupId });
            if (foundProducts.Count != productIds.Count)
                foreach (var productId in productIds)
                    if (!foundProducts.Any(p => p.Id == productId))
                        errors.Add(new ProductIdNotFoundError { Id = productId });
            
            if (errors.Any())
                return errors;

            var createTransactionResult = command.Type switch
            {
                TransactionType.In => 
                    Transaction.CreateInTypeTransaction(new CreateInTypeTransactionDto(
                        TransactionTime : command.TransactionTime,
                        StakeholderId: group!.Id,
                        Creator: new UserDto(Id: creator!.Id, IsAdmin: creator.IsAdmin),
                        Notes: command.Notes,
                        TransactionItems: command.TransactionItems.Select(item => 
                            item.ToCreateInTypeDomainDto()).ToList()
                    )),
                TransactionType.Out =>
                    Transaction.CreateOutTypeTransaction(new CreateOutTypeTransactionDto(
                        TransactionTime : command.TransactionTime,
                        StakeholderId: group!.Id,
                        Creator: new UserDto(Id: creator!.Id, IsAdmin: creator.IsAdmin),
                        Notes: command.Notes,
                        TransactionItems: command.TransactionItems.Select(item => 
                            item.ToCreateOutTypeDomainDto()).ToList(),
                        AssignedUser: assignedUser == null ? null : 
                            new UserDto(Id: assignedUser.Id, IsAdmin: assignedUser.IsAdmin)                        
                    )),
                _ => throw new ArgumentOutOfRangeException()
            };

            if (createTransactionResult.IsFailed())
            {
                errors.AddRange(createTransactionResult.GetError());
                return errors;
            }

            var sideEffects = createTransactionResult.GetData().Item2;
            var transaction = createTransactionResult.GetData().Item1;
            
            transactionRepository.Save(transaction);
            
            var productQuantityChangeResult = await productQuantityChangedEventHandler.Handle(
                sideEffects.ToList(), cancellationToken);
            if (productQuantityChangeResult.IsFailed())
                return productQuantityChangeResult.GetError()
                    .Select(e => (IBaseDomainError)e).ToList();
                
            await dbTransaction.CommitAsync(cancellationToken);
        }
        catch (Exception e)
        {
            logger.Error(e.Message);
            if (e.StackTrace != null) logger.Error(e.StackTrace);

            errors.Clear();
            errors.Add(new UnknownError());
            
            await dbTransaction.RollbackAsync(cancellationToken);
        }

        return errors;
    }
}