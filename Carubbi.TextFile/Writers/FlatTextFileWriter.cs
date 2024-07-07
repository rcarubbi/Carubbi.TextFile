using Carubbi.TextFile.Configuration;
using Carubbi.TextFile.Extensions;
using System.Text;

namespace Carubbi.TextFile.Writers;

public class FlatTextFileWriter
{
    public static async Task WriteDelimitedFileAsync<T>(List<T> models, string filePath, WritingOptions writingOptions, CancellationToken cancellationToken)
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
        throw new NotImplementedException();
    }

    private static string ParseDelimitedLine<T>(T model, char delimiter) 
    {
        var properties = typeof(T).GetDelimitedProperties();
        var values = new List<string>();
        foreach (var property in properties)
        {
            var value = property.GetValue(model)!.ToString() ?? string.Empty;
            values.Add(value);
        }
        return string.Join(delimiter, values);  
    }
}
