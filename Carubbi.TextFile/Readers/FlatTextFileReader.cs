using Carubbi.TextFile.Configuration;
using System.Text;

namespace Carubbi.TextFile.Readers;

public class FlatTextFileReader<T>(ReadingOptions readingOptions) : TextFileReaderBase where T : new()
{
    public List<T> ReadLines(string[] lines)
    {
        var result = new List<T>();
        foreach (var line in lines)
        {
            var instance = new T();
            ProcessLine(line, instance, readingOptions.Mode);
            result.Add(instance);
        }

        return result;
    }

    public async Task<List<T>> ReadFile(string filePath) 
    {
        var lines = (await File.ReadAllLinesAsync(filePath)).ToList();
        if (readingOptions.SkipHeader)
        {
            lines.Remove(lines[0]);
        }
        return ReadLines([.. lines]);
    }

    public async Task<List<T>> ReadFileInParallel(string filePath) 
    {
        int numberOfThreads = Environment.ProcessorCount;
        long fileSize = new FileInfo(filePath).Length;
        long bytesPerBatch = fileSize / numberOfThreads;

        var tasks = new List<Task<Batch<T>>>();

        using FileStream fs = new(filePath, FileMode.Open, FileAccess.Read);
        using StreamReader sr = new StreamReader(fs, Encoding.UTF8);   

        var headerOffset = await CalculateHeaderOffset(sr);

        for (int i = 0; i < numberOfThreads; i++)
        {
            var batchIndex = i;
            var task = ProcessBatch(filePath, numberOfThreads, bytesPerBatch, headerOffset, batchIndex);
            tasks.Add(task);
        }

        var batches = await Task.WhenAll(tasks);

        var ends = batches.SelectMany(x => x.Ends).ToList();

        foreach(var (batchIndex, lineIndex, line) in ends.DistinctBy(x => x.Content))
        {
            T model = new T();
            ProcessLine(line!, model, readingOptions.Mode);
            var batch = batches.Single(x => x.Index == batchIndex);
            if (lineIndex > batch.Models.Count - 1)
            {
                batch.Add(model);
            }
            else
            {
                batch.Insert(lineIndex, model);
            }
        }


        var items = batches.OrderBy(x => x.Index).SelectMany(x => x.Models).ToList();

        return items;
    }

    private async Task<Batch<T>> ProcessBatch(string filePath, int numberOfThreads, long bytesPerBatch, long headerOffset, int batchIndex)
    {
        var batch = new Batch<T>(batchIndex);
        
        await using FileStream fs = new(filePath, FileMode.Open, FileAccess.Read);
        using StreamReader reader = new(fs, Encoding.UTF8);
        
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

        long bytes = startPosition;
        var lineIndex = 0;
     
        while (!IsEndOfBatch(reader, endPosition, bytes))
        {
            line = await reader.ReadLineAsync();
            bytes += Encoding.UTF8.GetByteCount(line!) + Encoding.UTF8.GetByteCount(Environment.NewLine);

            if (lineIndex == 0 || IsEndOfBatch(reader, endPosition, bytes))
            {
                batch.AddEnd(lineIndex, line!);
            }
            else
            {
                var model = new T();
                ProcessLine(line!, model, readingOptions.Mode);
                batch.Add(model);
            }
            lineIndex++;
        }

        return batch;
    }

    private bool IsEndOfBatch(StreamReader reader, long endPosition, long bytes)
    {
        return bytes > endPosition || reader.EndOfStream;
    }

    private async Task<long> CalculateHeaderOffset(StreamReader streamReader)
    {
        long headerOffset = 0;

        if (readingOptions.SkipHeader)
        {
            string? headerLine = await streamReader.ReadLineAsync();

            if (headerLine != null)
            {
                headerOffset = Encoding.UTF8.GetByteCount(headerLine) + Encoding.UTF8.GetByteCount(Environment.NewLine);
            }
        }

        return headerOffset;
    }
}
