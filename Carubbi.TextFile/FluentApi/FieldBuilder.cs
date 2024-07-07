namespace Carubbi.TextFile.FluentApi;

public class FieldBuilder
{
    private FieldConfig _config;
    internal FieldBuilder(FieldConfig fieldConfig)
    {
        _config = fieldConfig;
    }

    public FieldBuilder InDelimitedOrder(int order)
    {
        _config.Order = order;
        return this;
    }

    public FieldBuilder InPositionalIndex(int startIndex, int length)
    {
        _config.StartIndex = startIndex;
        _config.Length = length;
        return this;
    }

    public void AsParentReference<T>()
    {
        _config.ParentReferenceType = typeof(T);
    }

    public void AsIdentifier()
    {
        _config.IsIdentifier = true;
    }
}

