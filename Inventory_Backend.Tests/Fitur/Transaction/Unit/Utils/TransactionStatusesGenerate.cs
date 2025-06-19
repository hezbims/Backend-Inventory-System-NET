using Inventory_Backend_NET.Fitur.Pengajuan.Domain.ValueObject;

namespace Inventory_Backend.Tests.Fitur.Transaction.Unit.Utils;

internal static class TransactionStatusesGenerate
{
    private static IEnumerable<TransactionStatus> BaseDatas => (TransactionStatus[]) 
        Enum.GetValues(typeof(TransactionStatus));

    private static IEnumerable<TransactionStatus> _baseDatasWithIgnored(params TransactionStatus[] ignoredStatuses) =>
        BaseDatas.Where(status => !ignoredStatuses.Contains(status));
    
    public static IEnumerable<object[]> NoWaiting =>
        _baseDatasWithIgnored(TransactionStatus.Waiting).ToArrayObject();
    
    public static IEnumerable<object[]> NoPrepared =>
        _baseDatasWithIgnored(TransactionStatus.Prepared).ToArrayObject();
    
    public static IEnumerable<object[]> NoConfirmed =>
        _baseDatasWithIgnored(TransactionStatus.Confirmed).ToArrayObject();
    
    public static IEnumerable<object[]> NoCanceled =>
        _baseDatasWithIgnored(TransactionStatus.Canceled).ToArrayObject();
    
    public static IEnumerable<object[]> NoCanceledAndRejected =>
        _baseDatasWithIgnored(TransactionStatus.Canceled, TransactionStatus.Rejected).ToArrayObject();
    
    public static IEnumerable<object[]> NoWaitingAndPrepared =>
        _baseDatasWithIgnored(TransactionStatus.Waiting, TransactionStatus.Prepared).ToArrayObject();

    public static IEnumerable<object[]> All => BaseDatas.ToArrayObject();
}

internal static class ToArrayObjectExtension
{
    internal static IEnumerable<object[]> ToArrayObject(this IEnumerable<TransactionStatus> statuses)
    {
        return statuses.Select(status => new object[] { status });
    }
}