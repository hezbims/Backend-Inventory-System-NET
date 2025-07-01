using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Inventory_Backend_NET.Common.Presentation.Validation;

public static class ValidationExtensions
{
    public static List<ValidationResult> Validate(this IValidatableObject objectToValidate)
    {
        List<ValidationResult> validationResults = [];
        Validator.TryValidateObject(
            objectToValidate,
            new ValidationContext(objectToValidate, null, null),
            validationResults,
            validateAllProperties: true);

        return validationResults;
    }
    
    public static bool TryValidate(this IValidatableObject objectToValidate, out List<ValidationResult> validationResults)
    {
        validationResults = Validate(objectToValidate);
        return validationResults.Count == 0;
    }

    public static void ReplaceAllErrors(
        this ModelStateDictionary modelState,
        IEnumerable<ValidationResult> validationResults
    )
    {
        modelState.Clear();
        foreach (var validationResult in validationResults)
            modelState.AddModelError(validationResult.MemberNames.First(), validationResult.ErrorMessage!);
    }

    public static IActionResult ValidationProblem(
        this ControllerBase controller,
        List<ValidationResult> validationResults)
    {
        controller.ModelState.ReplaceAllErrors(validationResults);
        return controller.ValidationProblem(controller.ModelState);
    }
}