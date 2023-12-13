using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Inventory_Backend_NET.Database;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Inventory_Backend_NET.UseCases.Barang;

public class ValidateBarangPropertyAvailability : ValidationAttribute
{
    public void Execute(
        int? id,
        Expression<Func<Models.Barang,bool>> predicate,
        MyDbContext db,
        string key,
        string errorMessage,
        ModelStateDictionary modelState
    )
    {
        var availableBarang = db.Barangs.FirstOrDefault(predicate);
        if (availableBarang == null) { return; }

        var detailErrorMessage = $"{errorMessage} ({availableBarang.Nama})";
        
        if (id == null)
        {
            modelState.AddModelError(key , detailErrorMessage);
        }
        else
        {
            if (id != availableBarang.Id)
            {
                modelState.AddModelError(key , detailErrorMessage);
            }
        }
    }
}