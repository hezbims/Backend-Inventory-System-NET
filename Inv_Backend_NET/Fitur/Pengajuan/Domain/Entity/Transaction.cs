using Inventory_Backend_NET.Common.Domain.Dto;
using Inventory_Backend_NET.Common.Domain.Event;
using Inventory_Backend_NET.Common.Domain.ValueObject;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.Transaction;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Exception;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Exception.CancelTransaction;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Exception.Common;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Exception.Common.TransactionItem;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Exception.ConfirmTransaction;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Exception.CreateTransaction;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Exception.PrepareTransaction;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Exception.RejectTransaction;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Exception.UpdateTransaction;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Mapper;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.ValueObject;
using Microsoft.IdentityModel.Tokens;

namespace Inventory_Backend_NET.Fitur.Pengajuan.Domain.Entity;

using CreateTransactionResult = Result<(Transaction, IReadOnlyList<ProductQuantityChangedEvent>), List<IBaseTransactionDomainError>>;
using PatchTransactionResult = Result<IReadOnlyList<ProductQuantityChangedEvent>, List<IBaseTransactionDomainError>>;
using CreateTransactionItemResults = List<Result<TransactionItem, List<TransactionItemError>>>;

internal sealed class Transaction
{
    public int Id { get; private set; }
    public long TransactionTime { get; private set; }
    public int StakeholderId { get; private set; }
    public TransactionType Type { get; private set; }
    public TransactionStatus Status { get; private set; }
    public int CreatorId { get; private set; } // User ID
    public int AssignedUserId { get; private set; }
    public IReadOnlyList<TransactionItem> TransactionItems { get; private set; }
    public string Notes { get; private set; }

    private Transaction(
        int id, TransactionType type, long transactionTime, int stakeholderId,
        TransactionStatus status, int creatorId, int assignedUserId, string notes,
        List<TransactionItem>? transactionItems = null)
    {
        Id = id;
        TransactionTime = transactionTime;
        StakeholderId = stakeholderId;
        Type = type;
        Status = status;
        CreatorId = creatorId;
        AssignedUserId = assignedUserId;
        Notes = notes;
        TransactionItems = transactionItems ?? [];
    }


    internal static CreateTransactionResult CreateNew(CreateNewTransactionDto dto)
    {
        List<IBaseTransactionDomainError> errors = [];
        CreateTransactionItemResults createTransactionItemResults;
        
        if (!dto.Creator.IsAdmin && dto.TransactionType == TransactionType.In)
            errors.Add(new UserNonAdminShouldNotCreateTransactionOfTypeInError());
        if (dto.TransactionItems.IsNullOrEmpty())
            errors.Add(new TransactionItemsShouldNotBeEmptyError());
        if (!dto.Creator.IsAdmin)
            createTransactionItemResults = dto.TransactionItems
                .Select((item, index) => TransactionItem.CreateNew(
                    productId: item.ProductId,
                    expectedQuantity: item.ExpectedQuantity,
                    preparedQuantity: null,
                    notes: item.Notes,
                    index: index
                )).ToList();
        else
        {
            if (dto.AssignedUser?.IsAdmin == true && dto.Creator.IsAdmin)
                errors.Add(new AdminMustNotAssignTransactionToAdminUserError());
            
            createTransactionItemResults = dto.TransactionItems
                .Select((item, index) => TransactionItem.CreateNew(
                    productId: item.ProductId,
                    expectedQuantity: item.ExpectedQuantity,
                    preparedQuantity: dto.TransactionType == TransactionType.In ?
                        item.ExpectedQuantity : item.PreparedQuantity,
                    notes: item.Notes,
                    index: index)).ToList();

        }
        errors.AddRange(createTransactionItemResults.GetErrors());
        
        if (!errors.IsNullOrEmpty())
            return new CreateTransactionResult.Failed(errors);

        int assigendUserId;
        // only admin can assign a transaction to another user when transaction type is out
        if (dto is
            {
                TransactionType: TransactionType.Out, 
                AssignedUser: not null, 
                Creator.IsAdmin: true
            })
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

        IReadOnlyList<TransactionItem> transactionItems = createTransactionItemResults.ToTransactionItems();
        Transaction newlyCreatedTransaction = new Transaction(
            id: 0,
            type: dto.TransactionType,
            transactionTime: dto.TransactionTime,
            stakeholderId: dto.StakeholderId,
            status: status,
            creatorId: dto.Creator.Id,
            assignedUserId: assigendUserId,
            notes: dto.Notes);
        List<ProductQuantityChangedEvent> sideEffects = newlyCreatedTransaction
            .ReplaceTransactionItems(transactionItems, hasSideEffects: dto.Creator.IsAdmin);
        
        return new CreateTransactionResult.Succeed((newlyCreatedTransaction, sideEffects));
    }

    public PatchTransactionResult PrepareTransaction(PrepareTransactionDto dto)
    {
        List<IBaseTransactionDomainError> errors = [];
        CreateTransactionItemResults createTransactionItemResults = [];
        
        if (!dto.Preparator.IsAdmin)
            errors.Add(new UserNonAdminShouldNotPrepareTransactionError());
        if (this.Status != TransactionStatus.Waiting)
            errors.Add(new OnlyWaitingTransactionCanBePreparedError());
        if (dto.TransactionItems.Count != TransactionItems.Count)
            errors.Add(new TransactionItemsSizeMustBeSameError());
        else
            createTransactionItemResults = dto.TransactionItems.Select((item, index) =>
                TransactionItem.CreateNew(
                    productId: TransactionItems[index].ProductId,
                    expectedQuantity: TransactionItems[index].ExpectedQuantity,
                    preparedQuantity: item.PreparedQuantity,
                    notes: TransactionItems[index].Notes,
                    index: index)).ToList();
        errors.AddRange(createTransactionItemResults.GetErrors());
        
        if (!errors.IsNullOrEmpty())
            return new PatchTransactionResult.Failed(errors);
        
        Status = TransactionStatus.Prepared; 
        Notes = dto.Notes;
        
        IReadOnlyList<ProductQuantityChangedEvent> sideEffects = ReplaceTransactionItems(
            newTransactionItems: createTransactionItemResults.ToTransactionItems(),
            hasSideEffects: true);
        
        return new PatchTransactionResult.Succeed(sideEffects);
    }
    
    public PatchTransactionResult CancelTransaction(CancelTransactionDto dto)
    {
        List<IBaseTransactionDomainError> errors = [];
        if (dto.Notes.IsNullOrEmpty())
            errors.Add(new CanNotCancelTransactionWithEmptyNotesError());
        if (this.AssignedUserId != dto.Cancelator.Id)
            errors.Add(new CanNotCancelOtherUserTransaction());
        else if (this.Status == TransactionStatus.Canceled)
            errors.Add(new TransactionCanNotCanceledTwiceError());
        else if (this.Status == TransactionStatus.Rejected)
            errors.Add(new RejectedTransactionCanNotCanceled());
        
        if (!errors.IsNullOrEmpty())
            return new PatchTransactionResult.Failed(errors);

        Status = TransactionStatus.Canceled;
        Notes = dto.Notes;
        
        List<ProductQuantityChangedEvent> sideEffects = GetSideEffectsOfReturnedPreparedTransactionItems();
        
        return new PatchTransactionResult.Succeed(sideEffects);
    }

    public PatchTransactionResult Reject(RejectTransactionDto dto)
    {
        if (!dto.Rejector.IsAdmin)
            return new PatchTransactionResult.Failed([
                new NonAdminIsNotAllowedToRejectTransactionError()]);
        
        List<IBaseTransactionDomainError> errors = [];
        CreateTransactionItemResults createTransactionItemResults = TransactionItems.Select((item, index) => 
            TransactionItem.CreateNew(
                productId: item.ProductId,
                expectedQuantity: item.ExpectedQuantity,
                preparedQuantity: null,
                notes: item.Notes,
                index: index)).ToList();
        if (this.Status is not (TransactionStatus.Waiting or TransactionStatus.Prepared))
            errors.Add(new OnlyWaitingAndPreparedTransactionCanBeRejectedError());
        if (dto.Notes.IsNullOrEmpty())
            errors.Add(new RejectionNotesMustNotBeEmptyError());
        errors.AddRange(createTransactionItemResults.GetErrors());
        
        if (!errors.IsNullOrEmpty())
            return new PatchTransactionResult.Failed(errors);

        this.Notes = dto.Notes;
        this.Status = TransactionStatus.Rejected;
        List<TransactionItem> newTransactionItems = createTransactionItemResults.ToTransactionItems();
        
        var sideEffects = ReplaceTransactionItems(newTransactionItems, hasSideEffects: true);
        return new PatchTransactionResult.Succeed(sideEffects);
    }

    public PatchTransactionResult UpdateTransaction(UpdateTransactionDto dto)
    {
        List<IBaseTransactionDomainError> errors = [];
        CreateTransactionItemResults createTransactionItemResults = [];
        
        if (!dto.Updater.IsAdmin)
        {
            if (this.Status != TransactionStatus.Waiting)
                errors.Add(new NonAdminCanNotUpdateNonWaitingTransactionError(
                    currentTransactionStatus: Status));
            if (dto.Updater.Id != this.CreatorId)
                errors.Add(new NonAdminCanOnlyUpdateTheirOwnTransactionError());   
            if (dto.Group!.IsSupplier)
                errors.Add(new NonAdminCanNotAssignSupplierGroupError());
            if (dto.TransactionItems.IsNullOrEmpty())
                errors.Add(new TransactionItemsShouldNotBeEmptyError());
            else
                createTransactionItemResults = dto.TransactionItems.Select((item, index) =>
                    TransactionItem.CreateNew(
                        productId: item.ProductId,
                        expectedQuantity: item.Quantity,
                        preparedQuantity: null,
                        notes: item.Notes,
                        index: index
                    )).ToList();
        }
        else
        {
            if (this.Status != TransactionStatus.Prepared)
                errors.Add(new AdminCanOnlyUpdatePreparedTransaction());
            if (dto.TransactionItems.Count != TransactionItems.Count)
                errors.Add(new TransactionItemsSizeMustBeSameError());
            else
                createTransactionItemResults = dto.TransactionItems.Select((item, index) =>
                    TransactionItem.CreateNew(
                        productId: TransactionItems[index].ProductId,
                        expectedQuantity: TransactionItems[index].ExpectedQuantity,
                        preparedQuantity: item.Quantity,
                        notes: TransactionItems[index].Notes,
                        index: index)).ToList();
        }
        errors.AddRange(createTransactionItemResults.GetErrors());
        
        if (!errors.IsNullOrEmpty())
            return new PatchTransactionResult.Failed(errors);

        if (!dto.Updater.IsAdmin)
        {
            TransactionTime = dto.TransactionTime!.Value;
            StakeholderId = dto.Group!.Id;
        }

        Notes = dto.Notes;

        List<ProductQuantityChangedEvent> sideEffects = ReplaceTransactionItems(
            newTransactionItems: createTransactionItemResults.ToTransactionItems(),
            hasSideEffects: dto.Updater.IsAdmin); // Side effect hanya ketika admin update prepared transaction
        
        return new PatchTransactionResult.Succeed(sideEffects);
    }

    public PatchTransactionResult ConfirmTransaction(
        ConfirmTransactionDto dto)
    {
        List<IBaseTransactionDomainError> errors = [];
        if (dto.User.IsAdmin)
            errors.Add(new AdminCanNotConfirmTransactionError());
        else
        {
            if (AssignedUserId != dto.User.Id)
                errors.Add(new NonAdminCanNotConfirmOtherUserTransaction());
            else if (Status != TransactionStatus.Prepared)
                errors.Add(new NonAdminCanOnlyConfirmPreparedTransaction(Status));
        }
        
        if (!errors.IsNullOrEmpty())
            return new PatchTransactionResult.Failed(errors);

        Status = TransactionStatus.Confirmed;
        Notes = dto.Notes;
        return new PatchTransactionResult.Succeed([]);
    }

    private List<ProductQuantityChangedEvent> ReplaceTransactionItems(
        IReadOnlyList<TransactionItem> newTransactionItems,
        bool hasSideEffects)
    {
        var oldTransactionItems = TransactionItems;
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

    List<ProductQuantityChangedEvent> GetSideEffectsOfReturnedPreparedTransactionItems()
    {
        return this.TransactionItems.ToProductQuantityChangedEvents(this.Type.GetInverse());
    }
}