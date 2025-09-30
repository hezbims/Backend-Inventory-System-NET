namespace Inventory_Backend_NET.Common.Presentation.Model;

internal interface IMyCustomValidatableObject
{
    internal bool TryValidate(out List<MyValidationError> validationResults);
}