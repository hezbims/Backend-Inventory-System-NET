namespace Inventory_Backend_NET.Common.Domain.ValueObject;

public enum TransactionType
{
    In, Out
}

public static class TransactionTypeExtensions
{
    public static TransactionType GetInverse(this TransactionType transactionType)
    {
        return transactionType == TransactionType.In ? TransactionType.Out : TransactionType.In;
    }
}