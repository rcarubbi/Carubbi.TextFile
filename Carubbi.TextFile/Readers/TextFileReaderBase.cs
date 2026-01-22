using Carubbi.TextFile.Configuration;
using Carubbi.TextFile.Extensions;
using System.Reflection;

namespace Carubbi.TextFile.Readers;

public abstract class TextFileReaderBase
{
    private static readonly IReadOnlyDictionary<Type, Func<string, object>> _converters = new Dictionary<Type, Func<string, object>>
    {
        [typeof(int)] = s => int.TryParse(s, out var v) ? v : throw new InvalidOperationException("Invalid integer value."),
        [typeof(decimal)] = s => decimal.TryParse(s, out var v) ? v : throw new InvalidOperationException("Invalid decimal value."),
        [typeof(float)] = s => float.TryParse(s, out var v) ? v : throw new InvalidOperationException("Invalid float value."),
        [typeof(DateTime)] = s => DateTime.TryParse(s, out var v) ? v : throw new InvalidOperationException("Invalid DateTime value."),
        [typeof(string)] = s => s
    };

    protected object? ConvertAndValidate(string valueString, PropertyInfo propertyInfo)
    {
        if (string.IsNullOrEmpty(valueString) && propertyInfo.IsNullable())
            return null;

        var type = propertyInfo.GetPropertyType();

        if (!_converters.TryGetValue(type, out var converter))
            throw new InvalidOperationException($"Unsupported data type: {type.Name}");

        return converter(valueString);
    }

    protected void ProcessLine(string line, object instance, ContentMode mode)
    {
        if (mode == ContentMode.Delimited)
        {
            ProcessDelimitedLine(line, instance);
        }
        else if (mode == ContentMode.Positional)
        {
            ProcessPositionalLine(line, instance);
        }
    }

    private void ProcessDelimitedLine<T>(string line, T instance) where T : class
    {
        var delimiter = instance.GetType().GetDelimiter() ?? throw new InvalidOperationException("Delimiter configuration is required for delimited mode."); 
       
        var values = line.Split(delimiter);

        foreach (var prop in instance.GetType().GetProperties())
        {
            var fieldOrder = prop.GetFieldOrder();

            if (fieldOrder.HasValue)
            {
                var fieldIndex = fieldOrder.Value - 1;
                if (fieldIndex < values.Length)
                {
                    try
                    {
                        string valueString = values[fieldIndex];
                        SetPropertyValue(instance, valueString, prop);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException($"Error converting field {prop.Name}: {ex.Message}");
                    }
                }
                else if (!prop.IsNullable())
                {
                    throw new InvalidOperationException($"Field {prop.Name} is not optional but missing in line.");
                }
            }
        }
    }

    private void SetPropertyValue(object instance, string valueString, PropertyInfo prop)
    {
        var value = ConvertAndValidate(valueString, prop);
        prop.SetValue(instance, value);
    }

    private void ProcessPositionalLine(string line, object instance)
    {
        foreach (var prop in instance.GetType().GetProperties())
        {
            var fieldStart = prop.GetStartIndex();
            var fieldLength = prop.GetFieldLength();

            if (fieldLength.HasValue && fieldStart.HasValue)
            {
                var endIndex = fieldStart.Value + fieldLength.Value;
                if (endIndex <= line.Length)
                {
                    var valueString = line.Substring(fieldStart.Value, fieldLength.Value).Trim();
                    SetPropertyValue(instance, valueString, prop);
                }
                else
                {
                    throw new InvalidOperationException($"Field {prop.Name} exceeds the length of the line.");
                }
            }
            else
            {
                throw new InvalidOperationException($"Field {prop.Name} doesn't have the required configuration to be mapped");
            }
        }
    }
}
