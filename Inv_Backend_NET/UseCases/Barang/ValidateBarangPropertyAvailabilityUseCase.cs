using System.Linq.Expressions;
using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.DTO.Barang;

namespace Inventory_Backend_NET.UseCases.Barang;

public class ValidateBarangPropertyAvailabilityUseCase
{

    private readonly MyDbContext _db;
    public ValidateBarangPropertyAvailabilityUseCase(
        MyDbContext db
    )
    {
        _db = db;
    }
    
    private KeyValuePair<string , string>? Validate(
        int? barangId,
        Expression<Func<Models.Barang,bool>> finder,
        string key,
        string errorMessage
    )
    {
        var availableBarang = _db.Barangs.FirstOrDefault(finder);
        if (availableBarang == null) { return null; }

        var detailErrorMessage = $"{errorMessage} (barang={availableBarang.Nama})";

        if (barangId != availableBarang.Id)
        {
            return new KeyValuePair<string, string>(key, detailErrorMessage);
        }

        return null;
    }
    

    public IDictionary<string , string> ValidatePropertyAvailability(
        int? barangId,
        ValidationProperty<string>? namaProperty,
        ValidationProperty<string>? kodeBarangProperty,
        ValidationProperty<RakDto>? rakProperty
    )
    {
        var validationResult = new Dictionary<string , string>();
        if (namaProperty != null)
        {
            var result = Validate(
                barangId: barangId,
                finder: barang => barang.Nama == namaProperty.Property,
                key: namaProperty.ErrorKey,
                errorMessage: namaProperty.ErrorMessage
            );
            if (result != null)
            {
                validationResult.Add(result.Value.Key , result.Value.Value);
            }
        }

        if (rakProperty != null)
        {
            var result = Validate(
                barangId: barangId,
                finder: barang => 
                    barang.NomorRak == rakProperty.Property.NomorRak &&
                    barang.NomorLaci == rakProperty.Property.NomorLaci &&
                    barang.NomorKolom == rakProperty.Property.NomorKolom,
                key: rakProperty.ErrorKey,
                errorMessage: rakProperty.ErrorMessage
            );
            if (result != null)
            {
                validationResult.Add(result.Value.Key , result.Value.Value);
            }
        }

        if (kodeBarangProperty != null)
        {
            var result = Validate(
                barangId: barangId,
                finder: barang => barang.KodeBarang == kodeBarangProperty.Property,
                key: kodeBarangProperty.ErrorKey,
                errorMessage: kodeBarangProperty.ErrorMessage
            );
            if (result != null)
            {
                validationResult.Add(result.Value.Key , result.Value.Value);
            }
        }

        return validationResult;
    }
}

public class ValidationProperty<T>
{
    public T Property { get; init; }
    public string ErrorKey { get; init; }
    public string ErrorMessage { get; init; }
    
    public ValidationProperty(
        T property,
        string errorKey,
        string errorMessage
    )
    {
        Property = property;
        ErrorKey = errorKey;
        ErrorMessage = errorMessage;
    }
}