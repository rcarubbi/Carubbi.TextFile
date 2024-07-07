namespace Carubbi.TextFile.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class DelimiterAttribute : Attribute
{
    public char Delimiter { get; }

    public DelimiterAttribute(char delimiter)
    {
        Delimiter = delimiter;
    }
}
