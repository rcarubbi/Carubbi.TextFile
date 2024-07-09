using Carubbi.TextFile.Configuration;
using Carubbi.TextFile.Extensions;
using System.Text;

namespace Carubbi.TextFile.Writers;

public class FlatTextFileWriter
{
    public static async Task WriteFileAsync<T>(List<T> models, string filePath, WritingOptions writingOptions, CancellationToken cancellationToken)
    {
        var fileContent = new StringBuilder();

        if (!string.IsNullOrWhiteSpace(writingOptions.Header))
        {
            fileContent.AppendLine(writingOptions.Header);
        }

        if (writingOptions.Mode == ContentMode.Delimited)
        {
            var delimiter = typeof(T).GetDelimiter() ?? throw new InvalidOperationException("Delimiter configuration is required for delimited mode.");

            foreach (var model in models)
            {
                fileContent.AppendLine(ParseDelimitedLine(model, delimiter));
            }
        }
        else
        {
            foreach (var model in models)
            {
                fileContent.AppendLine(ParsePositionalLine(model));
            }
        }

        await File.WriteAllTextAsync(filePath, fileContent.ToString(), cancellationToken); 
    }

    private static string? ParsePositionalLine<T>(T model)
    {
        var properties = typeof(T).GetPositionalProperties();
        var line = new StringBuilder();   
        foreach (var property in properties)
        {
            var length = property.GetFieldLength() ?? throw new InvalidOperationException("Field length is required to write positional layouts");
            var paddingDirection = property.GetPaddingDirection() ?? throw new InvalidOperationException("Padding direction is required to write positional layouts");
            var paddingChar = property.GetPaddingChar() ?? throw new InvalidOperationException("Padding char is required to write positional layouts");
            var stringValue = property.GetValue(model)?.ToString() ?? string.Empty;
            var filledValue = (paddingDirection == PaddingDirection.Left)
            ? stringValue.PadLeft(length, paddingChar)
            : stringValue.PadRight(length, paddingChar);

            line.Append(filledValue);
        }
        
        return line.ToString();
    }

    private static string ParseDelimitedLine<T>(T model, char delimiter) 
    {
        var properties = typeof(T).GetDelimitedProperties();
        var values = new List<string>();
        foreach (var property in properties)
        {
            var writeCustomParse = property.GetCustomParseMethod();
            var value = ((writeCustomParse != null)
                ? writeCustomParse.Invoke(model, [property.GetValue(model)])?.ToString() 
                : property.GetValue(model)!.ToString()) ?? string.Empty;

            values.Add(value);
        }
        return string.Join(delimiter, values);  
    }
}
