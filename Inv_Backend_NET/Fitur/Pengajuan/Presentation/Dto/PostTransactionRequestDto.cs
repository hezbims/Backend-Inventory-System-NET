using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Inventory_Backend_NET.Database.Models;
using Inventory_Backend_NET.Fitur._Constants;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.Transaction;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.TransactionItem;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.User;
using Microsoft.IdentityModel.Tokens;

namespace Inventory_Backend_NET.Fitur.Pengajuan.Presentation.Dto;

using TransType = Common.Domain.ValueObject.TransactionType;
using Key =  TransactionJsonFieldName;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class PostTransactionRequestDto : IValidatableObject
{
    [JsonIgnore]
    private UserCreator? Creator { get; set; } // late assign from http context

    public void SetUserCreator(User user)
    {
        Creator = new UserCreator(IsAdmin: user.IsAdmin, CreatorId: user.Id);
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
    public IList<TransactionItem>? TransactionItems { get; set; } // minimal satu biji

    public class TransactionItem
    {
        [JsonPropertyName(Key.ProductId)]
        public int? ProductId { get; set; } // required
        
        [JsonPropertyName(Key.ExpectedQuantity)]
        public int? ExpectedQuantity { get; set; } // required
        
        [JsonPropertyName("notes")]
        public string? Notes { get; set; }
    }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Creator is null)
            yield return new ValidationResult(
                Resources.Transaction.creator_is_empty, 
                 [ MyConstants.WrongContractErrorKey ]);

        if (TransactionTime is null)
            yield return new ValidationResult(
                Resources.Transaction.transaction_time_must_filled,
                [ Key.TransactionTime ]);

        if (GroupId is null)
            yield return new ValidationResult(
                Resources.Transaction.group_id_must_filled,
                [ Key.GroupId ]);

        if (Creator?.IsAdmin == true)
        {
            if (TransactionType is null) 
                yield return new ValidationResult(
                    Resources.Transaction.transaction_type_must_filled,
                    [Key.TransactionType]);
            else if (TransactionType != "IN" && TransactionType != "OUT")
                yield return new ValidationResult(
                    Resources.Transaction.transaction_type_must_be_in_or_out,
                    [MyConstants.WrongContractErrorKey]);
        }

        if (TransactionItems == null)
            yield return new ValidationResult(
                Resources.Transaction.transaction_items_is_null,
                [MyConstants.WrongContractErrorKey]);
        else if (TransactionItems.IsNullOrEmpty())
            yield return new ValidationResult(
                Resources.Transaction.transaction_items_must_have_1_item,
                [Key.TransactionItems]);
        else
        {
            int index = 0;
            foreach (var transactionItem in TransactionItems!)
            {
                if (transactionItem.ProductId is null)
                    yield return new ValidationResult(
                        String.Format(Resources.Transaction.transaction_item_product_id_is_not_filled, index),
                        [MyConstants.WrongContractErrorKey]);
                if (transactionItem.ExpectedQuantity is null)
                    yield return new ValidationResult(
                        String.Format(Resources.Transaction.expected_quantity_is_not_filled, index),
                        [MyConstants.WrongContractErrorKey]);
                index++;
            }
        }
    }

    public record UserCreator(int CreatorId, bool IsAdmin);

    public CreateNewTransactionDto ToDomainDto()
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
        return new CreateNewTransactionDto(
            TransactionType: transactionType,
            TransactionTime: this.TransactionTime!.Value,
            StakeholderId: this.GroupId!.Value,
            Creator: new UserDto(Id: Creator!.CreatorId, IsAdmin: Creator!.IsAdmin),
            Notes: this.Notes ?? string.Empty,
            TransactionItems: TransactionItems!.Select(item => 
                new CreateTransactionItemDto(
                    ProductId: item.ProductId!.Value, 
                    Quantity: item.ExpectedQuantity!.Value, 
                    Notes: item.Notes ?? string.Empty)).ToList()
            );
    }
}