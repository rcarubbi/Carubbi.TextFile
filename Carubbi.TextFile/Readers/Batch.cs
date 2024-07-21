namespace Carubbi.TextFile.Readers;

internal class Batch<T>(int index)
{
    private readonly List<T> _models = [];
    private readonly List<BatchEnd> _ends = [];

    public void Add(T model)
    {
        _models.Add(model); 
    }

    public void Insert(int index, T model)
    {
        _models.Insert(index, model);
    }

    public void AddEnd(int lineIndex, string content)
    {
        _ends.Add(new(Index, lineIndex, content));
    }

    public IReadOnlyCollection<T> Models => _models;

    public IReadOnlyCollection<BatchEnd> Ends => _ends;

    public int Index { get; } = index;

}

internal record BatchEnd(int BatchIndex, int LineIndex, string Content);
