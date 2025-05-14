using Inventory_Backend_NET.Common.Domain.Dto;
using Inventory_Backend_NET.Common.Domain.Event;
using Inventory_Backend_NET.Common.Domain.ValueObject;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Exception;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Mapper;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.ValueObject;
using Microsoft.IdentityModel.Tokens;

namespace Inventory_Backend_NET.Fitur.Pengajuan.Domain.Entity;

using CreateTransactionResult = Result<Transaction, List<IBaseTransactionDomainError>>;
using PatchTransactionResult = Result<IReadOnlyList<ProductQuantityChangedEvent>, List<IBaseTransactionDomainError>>;
public class Transaction
{
    public int Id { get; private set; }
    public long TransactionTime { get; private set; }
    public int StakeholderId { get; private set; }
    public TransactionType TransactionType { get; private set; }
    public TransactionStatus Status { get; private set; }
    public int CreatorId { get; private set; } // User ID

    public IReadOnlyList<TransactionItem> TransactionItems { get; private set; }

    private Transaction(
        int id, TransactionType transactionType, long transactionTime, int stakeholderId,
        TransactionStatus status, int creatorId, IReadOnlyList<TransactionItem> transactionItems)
    {
        Id = id;
        TransactionTime = transactionTime;
        StakeholderId = stakeholderId;
        Status = status;
        CreatorId = creatorId;
        TransactionItems = transactionItems;
    }


    public static CreateTransactionResult CreateNew(CreateNewTransactionDto dto)
    {
        if (!dto.Creator.IsAdmin && dto.TransactionType == TransactionType.In)
            return new CreateTransactionResult.Failed([
                new UserNonAdminShouldNotCreateTransactionOfTypeInError()
            ]);
        
        IReadOnlyList<TransactionItem> transactionItems = dto.TransactionItems.ToTransactionItemEntities();
        Transaction newlyCreatedTransaction = new Transaction(
            id: 0,
            transactionType: dto.TransactionType,
            transactionTime: dto.TransactionTime,
            stakeholderId: dto.StakeholderId,
            status: dto.Creator.IsAdmin ? TransactionStatus.Accepted : TransactionStatus.Waiting,
            creatorId: dto.Creator.UserAssignedId,
            transactionItems: transactionItems);
        
        return new CreateTransactionResult.Succeed(newlyCreatedTransaction);
    }

    public PatchTransactionResult AcceptTransaction(AcceptTransactionDto dto)
    {
        List<IBaseTransactionDomainError> errors = [];
        if (!dto.Acceptor.IsAdmin)
            errors.Add(new UserNonAdminShouldNotAcceptTransactionError());
        if (this.Status != TransactionStatus.Waiting)
            errors.Add(new OnlyWaitingTransactionCanBeAcceptedError());
        
        if (!errors.IsNullOrEmpty())
            return new PatchTransactionResult.Failed(errors);
        
        IReadOnlyList<TransactionItem> oldTransactionItems = dto.TransactionItems.ToTransactionItemEntities();

        Status = TransactionStatus.Accepted;
        TransactionItems = dto.TransactionItems.ToTransactionItemEntities();
        
        List<ProductQuantityChangedEvent> sideEffects = GenerateProductQuantityChangedEvents(
            oldTransactionItems: oldTransactionItems, newTransactionItems: TransactionItems);
        
        return new PatchTransactionResult.Succeed(sideEffects);
    }
    
    public PatchTransactionResult RejectTransaction(RejectTransactionDto dto)
    {
        List<IBaseTransactionDomainError> errors = [];
        if (!dto.Rejector.IsAdmin)
            errors.Add(new UserNonAdminShouldNotRejectTransactionError());
        if (this.Status != TransactionStatus.Waiting)
            errors.Add(new OnlyWaitingTransactionCanBeRejectedError());
        
        if (!errors.IsNullOrEmpty())
            return new PatchTransactionResult.Failed(errors);
        

        Status = TransactionStatus.Rejected;
        
        List<ProductQuantityChangedEvent> sideEffects = GenerateProductQuantityChangedEvents(
            oldTransactionItems: TransactionItems, newTransactionItems: []);
        
        return new PatchTransactionResult.Succeed(sideEffects);
    }

    public PatchTransactionResult UpdateTransaction(UpdateTransactionDto dto)
    {
        List<IBaseTransactionDomainError> errors = [];
        if (dto.UpdaterId != this.CreatorId)
            errors.Add(new UserCanOnlyUpdateTheirOwnTransactionError());
        if (!errors.IsNullOrEmpty())
            return new PatchTransactionResult.Failed(errors);

        TransactionTime = dto.TransactionTime;
        StakeholderId = dto.StakeholderId;
        IReadOnlyList<TransactionItem> oldTransactionItems = dto.TransactionItems.ToTransactionItemEntities();
        TransactionItems = dto.TransactionItems.ToTransactionItemEntities();

        List<ProductQuantityChangedEvent> sideEffects = [];
        GenerateProductQuantityChangedEvents(
            oldTransactionItems: oldTransactionItems, newTransactionItems: TransactionItems);
        
        return new PatchTransactionResult.Succeed(sideEffects);
    }

    private List<ProductQuantityChangedEvent> GenerateProductQuantityChangedEvents(
        IReadOnlyList<TransactionItem> oldTransactionItems,
        IReadOnlyList<TransactionItem> newTransactionItems)
    {
        List<ProductQuantityChangedEvent> events = [];
        
        // undo product quantity in old transaction items
        events.AddRange(oldTransactionItems.ToProductQuantityChangedEvents(TransactionType.GetInverse()));
        
        // apply product quantity in new transaction items
        events.AddRange(newTransactionItems.ToProductQuantityChangedEvents(TransactionType));

        return events;
    }
}