using System.ComponentModel.DataAnnotations;

namespace Inventory_Backend_NET.Fitur._Logic.UseCases;

[AttributeUsage(
    validOn: AttributeTargets.Property | AttributeTargets.Field,
    AllowMultiple = false
)]
public class AllowedFileExtensions : ValidationAttribute
{
    private readonly string[] _allowedExtensions;

    public AllowedFileExtensions(string[] allowedExtensions)
    {
        _allowedExtensions = allowedExtensions;
    }

    protected override ValidationResult? IsValid(
        object? value, 
        ValidationContext validationContext
    )
    {
        
        var file = value as IFormFile;
        if (file != null)
        {
            var extension = Path.GetExtension(file.FileName);
            if (!_allowedExtensions.Contains(extension))
            {
                return new ValidationResult(
                    errorMessage: $"Extension file yang diperbolehkan hanya : {string.Join(", " , _allowedExtensions)}"
                );
            }
        }
        return ValidationResult.Success;
    }
}