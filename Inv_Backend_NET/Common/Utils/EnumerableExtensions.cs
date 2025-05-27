namespace Inventory_Backend_NET.Common.Utils;

public static class EnumerableExtensions
{
    /// <summary>
    /// Memilih index-index dari item yang sesuai kriteria
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<int> SelectIndexWhere<T>(
        this IEnumerable<T> list,
        Predicate<T> predicate)
    {
        return list
            .Select((item, index) => (item, index))
            .Where(iterator => predicate(iterator.item))
            .Select(iterator => iterator.index)
            .ToList();
    }
}