using System.Linq.Expressions;
using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.DTO.Barang;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Inventory_Backend_NET.UseCases.Barang;

public class ValidateBarangPropertyAvailabilityUseCase
{

    private readonly MyDbContext _db;
    private readonly Func<ModelStateDictionary> _getModelState;
    public ValidateBarangPropertyAvailabilityUseCase(
        MyDbContext db,
        Func<ModelStateDictionary> getModelState
    )
    {
        _db = db;
        _getModelState = getModelState;
    }
    
    private void Validate(
        int? barangId,
        Expression<Func<Models.Barang,bool>> finder,
        string key,
        string errorMessage
    )
    {
        var availableBarang = _db.Barangs.FirstOrDefault(finder);
        if (availableBarang == null) { return; }

        var detailErrorMessage = $"{errorMessage} (barang={availableBarang.Nama})";
        Console.WriteLine($"ERROR KEY : {key} {availableBarang.Nama}");

        if (barangId != availableBarang.Id)
        {
            _getModelState().AddModelError(key, detailErrorMessage);
        }
    }
    

    public void ValidatePropertyAvailability(
        int? barangId,
        ValidationProperty<string>? namaProperty,
        ValidationProperty<string>? kodeBarangProperty,
        ValidationProperty<RakDto>? rakProperty
    )
    {
        if (namaProperty != null)
            Validate(
                barangId: barangId,
                finder: barang => barang.Nama == namaProperty.Property,
                key: namaProperty.ErrorKey,
                errorMessage: namaProperty.ErrorMessage
            );
        if (rakProperty != null)
            Validate(
                barangId: barangId,
                finder: barang => 
                    barang.NomorRak == rakProperty.Property.NomorRak &&
                    barang.NomorLaci == rakProperty.Property.NomorLaci &&
                    barang.NomorKolom == rakProperty.Property.NomorKolom,
                key: rakProperty.ErrorKey,
                errorMessage: rakProperty.ErrorMessage
            );
        if (kodeBarangProperty != null)
            Validate(
                barangId: barangId,
                finder: barang => barang.KodeBarang == kodeBarangProperty.Property,
                key: kodeBarangProperty.ErrorKey,
                errorMessage: kodeBarangProperty.ErrorMessage
            );
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