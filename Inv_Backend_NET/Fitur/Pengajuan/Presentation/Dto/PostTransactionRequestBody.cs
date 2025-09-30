using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Inventory_Backend_NET.Common.Presentation.Model;
using Inventory_Backend_NET.Fitur.Pengajuan.Application.Dto;

namespace Inventory_Backend_NET.Fitur.Pengajuan.Presentation.Dto;

using TransType = Common.Domain.ValueObject.TransactionType;
using Key =  TransactionJsonFieldName;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
internal sealed class PostTransactionRequestBody : IMyCustomValidatableObject
{
    [JsonIgnore]
    private UserCreator? Creator { get; set; } // late assign from http context

    public void SetUserCreator(bool isAdmin, int id)
    {
        Creator = new UserCreator(CreatorId: id, IsAdmin: isAdmin);
    }

    [JsonPropertyName("assigend_user_id")]
    public int? AssignedUserId { get; set; } // abaikan kalau non-admin, opsional kalau admin
    
    [JsonPropertyName(Key.TransactionType)]
    public string? TransactionType { get; set; } // required kalau admin
    
    [JsonPropertyName(Key.TransactionTime)]
    public long? TransactionTime { get; set; } // required
    
    [JsonPropertyName(Key.GroupId)]
    public int? GroupId { get; set; } // required
    
    [JsonPropertyName("notes")]
    public string? Notes { get; set; }
    
    [JsonPropertyName(Key.TransactionItems)]
    public IList<PostTransactionItemReqeustBody>? TransactionItems { get; set; } // minimal satu biji
    
    public bool TryValidate(out List<MyValidationError> errors)
    {
        errors = [];
        
        if (Creator is null)
            errors.Add(new MyValidationError(
                Code: "PRES_TRANSACTION_CREATOR_IS_EMPTY",
                Message: Resources.Transaction.creator_is_empty));

        if (TransactionTime is null)
            errors.Add(new MyValidationError(
                Code: "PRES_TRANSACTION_TIME_IS_EMPTY",
                Message: Resources.Transaction.transaction_time_must_filled));

        if (GroupId is null)
            errors.Add(new MyValidationError(
                Code: "PRES_TRANSACTION_GROUP_ID_IS_EMPTY",
                Message: Resources.Transaction.group_id_must_filled));
        
        if (TransactionType is null) 
            errors.Add(new MyValidationError(
                Code: "PRES_TRANSACTION_TYPE_IS_EMPTY",
                Message: Resources.Transaction.transaction_type_must_filled));
        else if (TransactionType != "IN" && TransactionType != "OUT")
            errors.Add(new MyValidationError(
                Code: "PRES_TRANASCTION_TYPE_IS_INVALID",
                Message: Resources.Transaction.transaction_type_must_be_in_or_out));

        if (TransactionItems == null)
            errors.Add(new MyValidationError(
                Code: "PRES_TRANSACTION_ITEMS_KEY_IS_NOT_EXIST",
                Resources.Transaction.transaction_items_is_null));
        else if (TransactionItems.Count == 0)
            errors.Add(new MyValidationError(
                Code: "PRES_TRANSACTION_ITEM_IS_EMPTY",
                Message: Resources.Transaction.transaction_items_must_have_1_item));
        else
        {
            int index = 0;
            foreach (var transactionItem in TransactionItems!)
            {
                if (transactionItem.ProductId is null)
                    errors.Add(new MyValidationError(
                        Code: "PRES_TRANSACTION_ITEM_PRODUCT_ID_IS_EMPTY",
                        Message: String.Format(Resources.Transaction.transaction_item_product_id_is_not_filled, index)));
                if (transactionItem.ExpectedQuantity is null)
                    errors.Add(new MyValidationError(
                        Code: "PRES_TRANSACTION_ITEM_EXPECTED_QUANTITY_MUST_EXIST",
                        Message: String.Format(Resources.Transaction.expected_quantity_is_not_filled, index)));
                
                if (TransactionType == "OUT" && 
                    Creator!.IsAdmin && 
                    transactionItem.PreparedQuantity is null)
                    errors.Add(new MyValidationError(
                        Code: "PRES_TRANSACTION_ITEM_PREPARED_QUANTITY_MUST_EXIST",
                        Message: String.Format(Resources.Transaction.prepared_quantity_must_filled, index)));
                index++;
            }
        }
        
        return errors.Count == 0;
    }

    private sealed record UserCreator(int CreatorId, bool IsAdmin);

    internal CreateTransactionCommand ToApplicationDto()
    {
        var transactionType = TransactionType switch
        {
            "IN" => TransType.In,
            "OUT" => TransType.Out,
            _ => throw new ArgumentOutOfRangeException(
                nameof(TransactionType), 
                TransactionType, 
                // ReSharper disable once LocalizableElement
                "Transaction type was not 'IN' or 'OUT'"),
        };
        return new CreateTransactionCommand(
            Type: transactionType,
            TransactionTime: this.TransactionTime!.Value,
            GroupId: this.GroupId!.Value,
            CreatorId: Creator!.CreatorId,
            Notes: this.Notes ?? string.Empty,
            AssignedUserId: this.AssignedUserId!.Value,
            TransactionItems: TransactionItems!.Select(item => 
                new CreateTransactionItemCommand(
                    ProductId: item.ProductId!.Value, 
                    ExpectedQuantity: item.ExpectedQuantity!.Value,
                    PreparedQuantity: item.PreparedQuantity,
                    Notes: item.Notes ?? string.Empty)).ToList()
            );
    }
}

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
internal sealed class PostTransactionItemReqeustBody
{
    [JsonPropertyName(Key.ProductId)]
    public int? ProductId { get; set; } // required
        
    [JsonPropertyName(Key.ExpectedQuantity)]
    public int? ExpectedQuantity { get; set; } // required
    
    [JsonPropertyName(Key.PreparedQuantity)]
    public int? PreparedQuantity { get; set; } // required kalau admin OUT
        
    [JsonPropertyName("notes")]
    public string? Notes { get; set; }
}