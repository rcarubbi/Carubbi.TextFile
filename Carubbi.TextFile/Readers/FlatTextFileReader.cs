using Carubbi.TextFile.Configuration;
using System.Collections.Concurrent;
using System.Text;

namespace Carubbi.TextFile.Readers;

public class FlatTextFileReader : TextFileReaderBase
{
    public static List<T> ReadLines<T>(string[] lines, ContentMode mode) where T : new()
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

    public static async Task<List<T>> ReadFile<T>(string filePath, ReadingOptions readingOptions) where T : new()
    {
        var lines = (await File.ReadAllLinesAsync(filePath)).ToList();
        if (readingOptions.SkipHeader)
        {
            lines.Remove(lines[0]);
        }
        return ReadLines<T>([.. lines], readingOptions.Mode);
    }

    public static async Task<List<T>> ReadFileInParallel<T>(string filePath, ReadingOptions readingOptions) where T : new()
    {
        int numberOfThreads = Environment.ProcessorCount;
        long fileSize = new FileInfo(filePath).Length;
        long bytesPerBatch = fileSize / numberOfThreads;

        var batches = new ConcurrentDictionary<int, List<T>>();
        var ends = new ConcurrentBag<(int batchIndex, int lineIndex, string content)>();
       
        var tasks = new List<Task>();
        Encoding encoding = Encoding.UTF8; 

        var headerOffset = await CalculateHeaderOffset(filePath, readingOptions, encoding);

        for (int i = 0; i < numberOfThreads; i++)
        {
            var batchIndex = i;
            var task = Task.Run(async () => await ProcessBatch(filePath, readingOptions, numberOfThreads, fileSize, bytesPerBatch, encoding, headerOffset, batchIndex, batches, ends));
            tasks.Add(task);
        }
        
        Task.WaitAll(tasks.ToArray());
        
        foreach(var (batchIndex, lineIndex, line) in ends.DistinctBy(x => x.content))
        {
            T model = new T();
            ProcessLine(line!, model, readingOptions.Mode);

            if (lineIndex > batches[batchIndex].Count - 1)
            {
                batches[batchIndex].Add(model);
            }
            else
            {
                batches[batchIndex].Insert(lineIndex, model);
            }
        }


        var items = batches.OrderBy(x => x.Key).SelectMany(x => x.Value).ToList();

        return items;
    }

    private static async Task ProcessBatch<T>(string filePath, ReadingOptions readingOptions, int numberOfThreads, long fileSize, long bytesPerBatch, Encoding encoding, long headerOffset, int batchIndex, ConcurrentDictionary<int, List<T>> batches, ConcurrentBag<(int batchIndex, int lineIndex, string content)> ends) where T : new()
    {
        using FileStream fs = new(filePath, FileMode.Open, FileAccess.Read);
        using StreamReader reader = new(fs, encoding);
        string? line = null;
        long startPosition = (batchIndex * bytesPerBatch);
        long endPosition = (batchIndex + 1) * bytesPerBatch;

        if (batchIndex == 0)
        {
            startPosition += headerOffset;
            fs.Seek(startPosition, SeekOrigin.Begin);
        }

        if (batchIndex > 0)
        {
            fs.Seek(startPosition, SeekOrigin.Begin);
            line = await reader.ReadLineAsync();
        }


        // Ensure the last chunk reads until the end of the file
        if (batchIndex == numberOfThreads - 1)
        {
            endPosition = fileSize;
        }

        long bytes = startPosition;
        var batch = new List<T>();
        var lineIndex = 0;
     
        while (!IsEndOfBatch(reader, endPosition, bytes))
        {
            line = await reader.ReadLineAsync();
            bytes += encoding.GetByteCount(line!) + encoding.GetByteCount(Environment.NewLine);

            if (lineIndex == 0 || IsEndOfBatch(reader, endPosition, bytes))
            {
                ends.Add((batchIndex, lineIndex, line!));
            }
            else
            {
                var model = new T();
                ProcessLine(line!, model, readingOptions.Mode);
                batch.Add(model);
            }
            lineIndex++;
        }

        batches.TryAdd(batchIndex, batch);
    }

    private static bool IsEndOfBatch(StreamReader reader, long endPosition, long bytes)
    {
        return bytes > endPosition || reader.EndOfStream;
    }

    private static async Task<long> CalculateHeaderOffset(string filePath, ReadingOptions readingOptions, Encoding encoding)
    {
        long headerOffset = 0;

        if (readingOptions.SkipHeader)
        {
            using FileStream fsHeader = new(filePath, FileMode.Open, FileAccess.Read);
            using StreamReader headerReader = new(fsHeader, encoding);

            string? headerLine = await headerReader.ReadLineAsync();

            if (headerLine != null)
            {
                headerOffset = encoding.GetByteCount(headerLine) + encoding.GetByteCount(Environment.NewLine);
            }
        }

        return headerOffset;
    }



}
