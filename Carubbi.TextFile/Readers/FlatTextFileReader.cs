using Carubbi.TextFile.Configuration;

namespace Carubbi.TextFile.Readers;

public class FlatTextFileReader : TextFileReaderBase
{
    public static List<T> ReadLines<T>(string[] lines, ContentMode mode) where T : class, new()
    {
        var result = new List<T>();
        foreach (var line in lines)
        {
            var instance = new T();
            ProcessLine(line, instance, mode);
            result.Add(instance);
        }

        return result;
    }

    public static List<T> ReadFile<T>(string filePath, ReadingOptions readingOptions) where T : class, new()
    {
        var lines = File.ReadAllLines(filePath).ToList();
        if (readingOptions.SkipHeader)
        {
            lines.Remove(lines[0]);
        }
        return ReadLines<T>([.. lines], readingOptions.Mode);
    }

}
