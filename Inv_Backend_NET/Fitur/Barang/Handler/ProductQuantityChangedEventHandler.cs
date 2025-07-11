using Inventory_Backend_NET.Common.Domain.Dto;
using Inventory_Backend_NET.Common.Domain.Event;
using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Fitur.Barang.Exception;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Backend_NET.Fitur.Barang.Handler;

using Barang = Database.Models.Barang;
using HandleResult = Result<object?, List<ProductQuantityNotSufficientError>>;

internal sealed class ProductQuantityChangedEventHandler(
    MyDbContext dbContext)
{
    /// <summary>
    /// Mengurangi atau menambahkan stock dari product sesuai dengan <c>events</c>.
    /// Setelah method <c>Handle</c> berhasil, anda harus menggunakan <c>SaveChanges</c>/<c>SaveChangesAsync</c>
    /// pada DbContext untuk meng-apply perubahan
    /// </summary>
    /// <param name="events"></param>
    /// <param name="cancellationToken"></param>
    /// <returns><c>true</c> jika berhasil, <c>false</c> jika gagal dengan <c>result</c> berisi error</returns>
    internal async Task<HandleResult> Handle(
        List<ProductQuantityChangedEvent> events,
        CancellationToken cancellationToken)
    {
        if (!events.Any())
            return new HandleResult.Succeed(null);
        
        List<int> productIds = events.Select(e => e.ProductId).ToList();
        List<Barang> targetProducts = await dbContext.Barangs
            .Where(product => productIds.Contains(product.Id))
            .ToListAsync(cancellationToken);

        var pairProductEvents = targetProducts
            // Buat pasangan barang dengan taken quantitynya
            .Select(product =>
            {
                var currentProductEvent = events
                    .First(e => e.ProductId == product.Id);
                return new
                {
                    product,
                    takenQuantity = currentProductEvent.Quantity,
                };
            }).ToList();
        
        var insufficientStockProducts = pairProductEvents
            // Filter pasangan barang yang takenQuantity-nya lebih dari curentStock-nya
            .Where(item => 
                item.takenQuantity > item.product.CurrentStock)
            .ToList();

        if (insufficientStockProducts.Any())
            return new HandleResult.Failed(insufficientStockProducts
                .Select(item => new ProductQuantityNotSufficientError
                {
                    CurrentQuantity = item.product.CurrentStock,
                    TakenQuantity = item.takenQuantity,
                    ProductId = item.product.Id,
                })
                .ToList());

        List<Barang> updatedProducts = pairProductEvents.Select(item =>
        {
            Barang product = item.product;
            product.CurrentStock -= item.takenQuantity;
            return product;
        }).ToList();
        
        dbContext.AddRange(updatedProducts);
        return new HandleResult.Succeed(null);
    }
}