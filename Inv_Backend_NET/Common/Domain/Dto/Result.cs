namespace Inventory_Backend_NET.Common.Domain.Dto;

public abstract record Result<T , TE>
{
    public sealed record Succeed(T Data) : Result<T, TE>;
    public sealed record Failed(TE Error) : Result<T , TE>;
}