using Carubbi.TextFile.Configuration;
using Carubbi.TextFile.Extensions;
using System.Text;

namespace Carubbi.TextFile.Writers;

public class FlatTextFileWriter<T>(WritingOptions writingOptions)
{
    public async Task WriteFileAsync(List<T> models, string filePath, CancellationToken cancellationToken)
    {
        await using var writer = new StreamWriter(filePath, false, Encoding.Default, bufferSize: CalculateBufferSize());
       
        if (!string.IsNullOrWhiteSpace(writingOptions.Header))
        {
            await writer.WriteLineAsync(writingOptions.Header);
        }

        if (writingOptions.Mode == ContentMode.Delimited)
        {
            await WriteDelimitedLines(models, writer, cancellationToken);
        }
        else
        {
            await WritePositionalLines(models, writer, cancellationToken);
        }


    }

    private async Task WritePositionalLines(List<T> models, StreamWriter writer, CancellationToken cancellationToken)
    {
        if (writingOptions.IgnoreLastLineBreak)
        {
            for (var i = 0; i < models.Count; i++)
            {
                if (models.Count - 1 == i)
                {
                    await writer.WriteAsync(ParsePositionalLine(models[i]), cancellationToken);
                }
                else
                {
                    await writer.WriteLineAsync(ParsePositionalLine(models[i]), cancellationToken);
                }
            }
        }
        else
        {
            foreach(var model in models)
            {
                await writer.WriteLineAsync(ParsePositionalLine(model), cancellationToken);
            }
        }
    }

    private async Task WriteDelimitedLines(List<T> models, StreamWriter writer, CancellationToken cancellationToken)
    {
        var delimiter = typeof(T).GetDelimiter() ?? throw new InvalidOperationException("Delimiter configuration is required for delimited mode.");


        if (writingOptions.IgnoreLastLineBreak)
        {
            for (var i = 0; i < models.Count; i++)
            {
                if (models.Count - 1 == i)
                {
                    await writer.WriteAsync(ParseDelimitedLine(models[i], delimiter), cancellationToken);
                }
                else
                {
                    await writer.WriteLineAsync(ParseDelimitedLine(models[i], delimiter), cancellationToken);
                }
            }
        }
        else
        {
            foreach (var model in models)
            {
                await writer.WriteLineAsync(ParseDelimitedLine(model, delimiter), cancellationToken);
            }
        }
    }

    private int CalculateBufferSize()
    {
        const int maxMemory = 32 * 1024 * 1024;
        MemoryInfo.GetMemoryStatus(out _, out var availableMemoryMB);

        var memoryReserved = availableMemoryMB / 2 * 1024 * 1024;
        int memory = memoryReserved > maxMemory ? maxMemory : (int)memoryReserved;
        
        return memory;
    }

    private StringBuilder ParsePositionalLine(T model)
    {
        var properties = typeof(T).GetPositionalProperties();
        var lineContent = new StringBuilder();   
        foreach (var property in properties)
        {
            var length = property.GetFieldLength() ?? throw new InvalidOperationException("Field length is required to write positional layouts");
            var paddingDirection = property.GetPaddingDirection() ?? throw new InvalidOperationException("Padding direction is required to write positional layouts");
            var paddingChar = property.GetPaddingChar() ?? throw new InvalidOperationException("Padding char is required to write positional layouts");
            var (modelCustomParse, fluentCustomParse) = property.GetCustomParseMethod();
            
            var value = ((modelCustomParse != null)
                 ? modelCustomParse.Invoke(model, [property.GetValue(model)])?.ToString()
                 : (fluentCustomParse != null)
                    ? fluentCustomParse(property.GetValue(model) ?? string.Empty) 
                    : property.GetValue(model)!.ToString()) ?? string.Empty;

            var filledValue = (paddingDirection == PaddingDirection.Left)
            ? value.PadLeft(length, paddingChar)
            : value.PadRight(length, paddingChar);

            lineContent.Append(filledValue);
        }
        
        return lineContent;
    }

    private StringBuilder ParseDelimitedLine(T model, char delimiter) 
    {
        var properties = typeof(T).GetDelimitedProperties();
        var lineContent = new StringBuilder();
        var values = new List<string>();
        foreach (var property in properties)
        {
            var (modelCustomParse, fluentCustomParse) = property.GetCustomParseMethod();

            var value = ((modelCustomParse != null)
               ? modelCustomParse.Invoke(model, [property.GetValue(model)])?.ToString()
               : (fluentCustomParse != null)
                  ? fluentCustomParse(property.GetValue(model) ?? string.Empty)
                  : property.GetValue(model)!.ToString()) ?? string.Empty;

            values.Add(value);
        }
        lineContent.AppendJoin(delimiter, [.. values]);
        return lineContent;  
    }
}
