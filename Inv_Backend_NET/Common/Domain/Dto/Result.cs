namespace Inventory_Backend_NET.Common.Domain.Dto;

public abstract record Result<T , TE>
{
    public sealed record Succeed(T Data) : Result<T, TE>;
    public sealed record Failed(TE Error) : Result<T , TE>;

    public bool IsSucceed() => this is Succeed;
    public bool IsFailed() => this is Failed;
    
    public T GetData() =>
        this is Succeed
            ? ((this as Succeed)!).Data
            : throw new TypeAccessException("Trying to access data in failed result");

    public TE GetError() =>
        this is Failed ? ((this as Failed)!).Error :
            throw new TypeAccessException("Trying to access error in succeed result");
    
}