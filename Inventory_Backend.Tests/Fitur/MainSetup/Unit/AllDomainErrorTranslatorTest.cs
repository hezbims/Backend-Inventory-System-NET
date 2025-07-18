using System.Reflection;
using Inventory_Backend_NET.Common.Domain.Exception;
using Inventory_Backend_NET.Startup.Service;
using Xunit.Abstractions;

namespace Inventory_Backend.Tests.Fitur.MainSetup.Unit;

public sealed class AllDomainErrorTranslatorTest(
    #pragma warning disable CS9113 // Parameter is unread.
    ITestOutputHelper output)
{
    [Fact]
    public void All_Domain_Error_Should_Be_Translated_Uniquely_By_Domain_Error_Translator()
    {
        AllDomainErrorTranslatorImpl translator = new();
        var baseDomainErrorType = typeof(IBaseDomainError);
        var domainErrorTypes = Assembly.GetAssembly(baseDomainErrorType)!
            .GetTypes()
            .Where(t => 
                t is { IsAbstract: false, IsInterface: false } && 
                baseDomainErrorType.IsAssignableFrom(t))
            .ToList();

        Dictionary<String, int> errorCodeCounter = new();

        foreach (var errorType in domainErrorTypes)
        {
            var constructor = errorType
                .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .OrderBy(c => c.GetParameters().Length)
                .FirstOrDefault();

            // output.WriteLine($"type : {errorType.FullName}");
            Assert.NotNull(constructor);

            var parameters = constructor.GetParameters()
                .Select(p => GetDefault(p.ParameterType))
                .ToArray();

            var domainErrorInstance = (IBaseDomainError) constructor.Invoke(parameters);

            var validationError = translator.Translate(domainErrorInstance);
            
            errorCodeCounter.TryAdd(validationError.Code, 0);
            errorCodeCounter[validationError.Code]++;
        }

        foreach (var entry in errorCodeCounter)
            Assert.Equal(1, entry.Value);
    }
    
    private object? GetDefault(Type type)
    {
        if (type == typeof(string))
            return "default";
        if (type == typeof(int) || type == typeof(long) || type == typeof(double))
            return 0;
        if (type == typeof(Guid))
            return Guid.Empty;
        if (type == typeof(DateTime))
            return DateTime.MinValue;
        if (type.IsEnum)
            return Enum.GetValues(type).GetValue(0);
        if (type.IsValueType)
            return Activator.CreateInstance(type);

        return null; // For reference types
    }
}