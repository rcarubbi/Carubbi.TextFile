namespace Carubbi.TextFile.FluentApi;

public class RecordTypeBuilder<T> where T : class
{
    private readonly TextFileRecordTypeConfiguration<T> _config;

    public RecordTypeBuilder(TextFileRecordTypeConfiguration<T> config)
    {
        _config = config;
    }

    public TextFileRecordTypeConfiguration<T> InDelimitedOrder(int order)
    {
        _config.RecordTypeOrder = order;
        return _config;
    }

    public TextFileRecordTypeConfiguration<T> InPositionalIndex(int startIndex, int length)
    {
        _config.RecordTypeStartIndex = startIndex;
        _config.RecordTypeLength = length;
        return _config;
    }
}

