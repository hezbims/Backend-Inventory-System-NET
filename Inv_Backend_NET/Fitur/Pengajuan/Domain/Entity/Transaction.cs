using Inventory_Backend_NET.Common.Domain.Dto;
using Inventory_Backend_NET.Common.Domain.Event;
using Inventory_Backend_NET.Common.Domain.ValueObject;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Exception;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Mapper;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.ValueObject;
using Microsoft.IdentityModel.Tokens;

namespace Inventory_Backend_NET.Fitur.Pengajuan.Domain.Entity;

using CreateTransactionResult = Result<(Transaction, IReadOnlyList<ProductQuantityChangedEvent>), List<IBaseTransactionDomainError>>;
using PatchTransactionResult = Result<IReadOnlyList<ProductQuantityChangedEvent>, List<IBaseTransactionDomainError>>;
public class Transaction
{
    public int Id { get; private set; }
    public long TransactionTime { get; private set; }
    public int StakeholderId { get; private set; }
    public TransactionType Type { get; private set; }
    public TransactionStatus Status { get; private set; }
    public int CreatorId { get; private set; } // User ID
    public int AssignedUserId { get; private set; }
    public IReadOnlyList<TransactionItem> TransactionItems { get; private set; } = [];

    private Transaction(
        int id, TransactionType type, long transactionTime, int stakeholderId,
        TransactionStatus status, int creatorId, int assignedUserId)
    {
        Id = id;
        TransactionTime = transactionTime;
        StakeholderId = stakeholderId;
        Type = type;
        Status = status;
        CreatorId = creatorId;
        AssignedUserId = assignedUserId;
    }


    public static CreateTransactionResult CreateNew(CreateNewTransactionDto dto)
    {
        List<IBaseTransactionDomainError> errors = [];
        if (!dto.Creator.IsAdmin && dto.TransactionType == TransactionType.In)
            errors.Add(new UserNonAdminShouldNotCreateTransactionOfTypeInError());
        if (dto.TransactionItems.IsNullOrEmpty())
            errors.Add(new TransactionItemsShouldNotBeEmptyError());
        if (!errors.IsNullOrEmpty())
            return new CreateTransactionResult.Failed(errors);

        int assigendUserId;
        // only admin can assign a transaction to another user when transaction type is out
        if (dto.TransactionType == TransactionType.Out && 
            dto.AssignedUser != null && 
            dto.Creator.IsAdmin)
            assigendUserId = dto.AssignedUser.Id;
        else
            assigendUserId = dto.Creator.Id;
        
        TransactionStatus status;
        if (!dto.Creator.IsAdmin)
            status = TransactionStatus.Waiting;
        else
        {
            if (dto.TransactionType == TransactionType.In)
                status = TransactionStatus.Confirmed;
            else
            {
                if (dto.AssignedUser?.IsAdmin == false)
                    status = TransactionStatus.Prepared;
                else
                    status = TransactionStatus.Confirmed;
            }
        }
        
        IReadOnlyList<TransactionItem> transactionItems = dto.TransactionItems.ToTransactionItemEntities();
        Transaction newlyCreatedTransaction = new Transaction(
            id: 0,
            type: dto.TransactionType,
            transactionTime: dto.TransactionTime,
            stakeholderId: dto.StakeholderId,
            status: status,
            creatorId: dto.Creator.Id,
            assignedUserId: assigendUserId);
        List<ProductQuantityChangedEvent> sideEffects = newlyCreatedTransaction
            .ReplaceTransactionItems(transactionItems, hasSideEffects: dto.Creator.IsAdmin);
        
        return new CreateTransactionResult.Succeed((newlyCreatedTransaction, sideEffects));
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
        
        Status = TransactionStatus.Prepared;
        
        List<ProductQuantityChangedEvent> sideEffects = ReplaceTransactionItems(
            dto.TransactionItems.ToTransactionItemEntities(),
            hasSideEffects: true);
        
        return new PatchTransactionResult.Succeed(sideEffects);
    }
    
    public PatchTransactionResult CancelTransaction(RejectTransactionDto dto)
    {
        List<IBaseTransactionDomainError> errors = [];
        if (!dto.Rejector.IsAdmin)
            errors.Add(new UserNonAdminShouldNotRejectTransactionError());
        if (this.Status != TransactionStatus.Waiting)
            errors.Add(new OnlyWaitingTransactionCanBeRejectedError());
        
        if (!errors.IsNullOrEmpty())
            return new PatchTransactionResult.Failed(errors);
        

        Status = TransactionStatus.Canceled;
        
        List<ProductQuantityChangedEvent> sideEffects = GenerateProductQuantityChangedEvents(
            oldTransactionItems: TransactionItems);
        
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
            oldTransactionItems: oldTransactionItems);
        
        return new PatchTransactionResult.Succeed(sideEffects);
    }

    private List<ProductQuantityChangedEvent> ReplaceTransactionItems(
        IReadOnlyList<TransactionItem> newTransactionItems,
        bool hasSideEffects)
    {
        IReadOnlyList<TransactionItem> oldTransactionItems = TransactionItems;
        TransactionItems = newTransactionItems;
        
        List<ProductQuantityChangedEvent> events = [];
        if (hasSideEffects)
        {
            // undo product quantity in old transaction items
            events.AddRange(oldTransactionItems.ToProductQuantityChangedEvents(Type.GetInverse()));

            // apply product quantity in current transaction items
            events.AddRange(TransactionItems.ToProductQuantityChangedEvents(Type));
        }

        return events;
    }
    
    private List<ProductQuantityChangedEvent> GenerateProductQuantityChangedEvents(
        IReadOnlyList<TransactionItem> oldTransactionItems)
    {
        List<ProductQuantityChangedEvent> events = [];
        
        // undo product quantity in old transaction items
        events.AddRange(oldTransactionItems.ToProductQuantityChangedEvents(Type.GetInverse()));
        
        // apply product quantity in current transaction items
        events.AddRange(TransactionItems.ToProductQuantityChangedEvents(Type));

        return events;
    }
}