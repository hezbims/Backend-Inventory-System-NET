using Inventory_Backend_NET.Common.Presentation.Model;
using Inventory_Backend_NET.Fitur.Barang.Exception;

namespace Inventory_Backend_NET.Fitur.Barang.ErrorTranslator;

internal sealed class ProductDomainErrorTranslator
{
    internal MyValidationError Translate(BaseProductDomainError error)
    {
        return error switch
        {
            ProductIdNotFoundError => new MyValidationError(
                Code: "DOM_PRODUCT_ID_NOT_FOUND", Message: Resources.Product.id_not_found),
            ProductQuantityNotSufficientError => new MyValidationError(
                Code: "DOM_PRODUCT_STOCK_NOT_SUFFICIENT", Message: Resources.Product.stock_not_sufficient),
            _ => throw new ArgumentOutOfRangeException(
                $"Product domain error can not translated : {error.GetType().FullName}")
        };
    }
}