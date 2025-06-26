using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Inventory_Backend_NET.Database.Models;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.ValueObject;
using Inventory_Backend_NET.Fitur.Pengajuan.Infrastructure.EF.Config;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Backend_NET.Fitur.Pengajuan.Infrastructure.EF;

using Pengaju = Database.Models.Pengaju;

[EntityTypeConfiguration(typeof(TransactionEfConfig))]
[Table("Transactions")]
public class TransactionEf
{
    public required int Id { get; init; }
    
    public required long TransactionTime { get; init; }
    
    public required int GroupId { get; init; }
    public Pengaju? Group { get; init; }
    
    public required TransactionStatus Status { get; init; }
    
    public required int CreatorId { get; init; } // User ID
    public User? Creator { get; init; }
    
    public required int AssignedUserId { get; init; }
    public User? AssignedUser { get; init; }

    public IReadOnlyList<TransactionItemEf> TransactionItems { get; init; } = [];
    
    [MaxLength(1500)]
    public required string Notes { get; init; } = string.Empty;

    // Supaya Transaction yang statusnya waiting, dilihat paling atas oleh admin
    public required int Priorities { get; init; }
    
    public required long UpdatedAt { get; init; }
    
    public required long CreatedAt { get; init; }
}