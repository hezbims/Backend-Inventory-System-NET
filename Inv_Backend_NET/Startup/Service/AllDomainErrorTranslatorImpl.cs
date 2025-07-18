using Inventory_Backend_NET.Common.Domain.Exception;
using Inventory_Backend_NET.Common.Presentation.Model;
using Inventory_Backend_NET.Common.Presentation.Service;
using Inventory_Backend_NET.Fitur.Autentikasi.Domain.Exception;
using Inventory_Backend_NET.Fitur.Autentikasi.ErrorTranslator;
using Inventory_Backend_NET.Fitur.Barang.ErrorTranslator;
using Inventory_Backend_NET.Fitur.Barang.Exception;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Exception;
using Inventory_Backend_NET.Fitur.Pengajuan.Presentation.ErrorTranslator;

namespace Inventory_Backend_NET.Startup.Service;

internal sealed class AllDomainErrorTranslatorImpl : IAllDomainErrorTranslator
{
    private readonly ProductDomainErrorTranslator _productDomainErrorTranslator = new ();
    private readonly TransactionDomainErrorTranslator _transactionDomainErrorTranslator = new ();
    private readonly AuthDomainErrorTranslator _authDomainErrorTranslator = new ();
    
    public MyValidationError Translate(IBaseDomainError error)
    {
        return error switch
        {
            BaseProductDomainError productError => _productDomainErrorTranslator.Translate(productError),
            IBaseTransactionDomainError transactionError => _transactionDomainErrorTranslator.Translate(transactionError),
            BaseAuthError authError => _authDomainErrorTranslator.Translate(authError),
            UnknownError => new MyValidationError(
                Code: "DOM_UNKNOWN_ERROR",
                Message: Resources.Other.unknown_error),
            _ => throw new ArgumentOutOfRangeException(nameof(error), error, null)
        };
    }
}