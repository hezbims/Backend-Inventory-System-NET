using Inventory_Backend_NET.Common.Domain.Exception;
using Inventory_Backend_NET.Common.Presentation.Model;

namespace Inventory_Backend_NET.Common.Presentation.Service;

internal interface IAllDomainErrorTranslator
{
    internal MyValidationError Translate(IBaseDomainError error);
}