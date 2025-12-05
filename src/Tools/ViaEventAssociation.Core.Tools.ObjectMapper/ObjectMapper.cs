using System;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;

namespace ViaEventAssociation.Core.Tools.ObjectMapper;

// Object Mapper by chatty (no dynamic types, so that compiler can complain if types mismatch etc.)
public class ObjectMapper(IServiceProvider serviceProvider) : IObjectMapper
{
    public TOutput Map<TOutput>(object input)
        where TOutput : class
    {
        if (input is null) throw new ArgumentNullException(nameof(input));

        var inputType = input.GetType();
        var outputType = typeof(TOutput);

        // Try specific mapping config first
        var configType = typeof(IMappingConfig<,>).MakeGenericType(inputType, outputType);
        var mappingConfig = serviceProvider.GetService(configType);

        if (mappingConfig is not null)
        {
            // Call Map(input) via reflection
            var mapMethod = configType.GetMethod("Map");
            if (mapMethod is null)
            {
                throw new InvalidOperationException(
                    $"Mapping config {configType.Name} does not define a Map method.");
            }

            var mapped = mapMethod.Invoke(mappingConfig, new[] { input });

            if (mapped is null)
            {
                throw new InvalidOperationException(
                    $"Mapping config {configType.Name} returned null for {inputType.Name} -> {outputType.Name}.");
            }

            return (TOutput)mapped;
        }

        // Fallback: JSON serialize + deserialize
        var json = JsonSerializer.Serialize(input);
        var result = JsonSerializer.Deserialize<TOutput>(json);

        if (result is null)
        {
            throw new InvalidOperationException(
                $"Failed to map from {inputType.Name} to {outputType.Name} using JSON fallback.");
        }

        return result;
    }
}
