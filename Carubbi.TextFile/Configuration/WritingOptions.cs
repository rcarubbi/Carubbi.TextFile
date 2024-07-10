namespace Carubbi.TextFile.Configuration;

public class WritingOptions
{
    public string? Header { get; set; }

    public ContentMode Mode { get; set; }

    public bool IgnoreLastLineBreak { get; set; }
}
