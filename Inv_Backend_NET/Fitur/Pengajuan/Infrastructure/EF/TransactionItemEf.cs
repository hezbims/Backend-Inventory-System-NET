using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory_Backend_NET.Fitur.Pengajuan.Infrastructure.EF;

using Barang = Database.Models.Barang;

[Table("TransactionItems")]
public class TransactionItemEf
{
    public required int Id { get; init; }
    
    public required int TransactionId { get; init; }
    public TransactionEf? Transaction { get; init; }
    
    public required int ProductId { get; init; }
    
    public Barang? Product { get; init; }
    
    public required int ExpectedQuantity { get; init; }
    
    public required int? PreparedQuantity { get; init; }

    [MaxLength(255)]
    public required string Notes { get; init; } = string.Empty;
}