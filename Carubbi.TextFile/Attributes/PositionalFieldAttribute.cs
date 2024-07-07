namespace Carubbi.TextFile.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
public class PositionalFieldAttribute : Attribute
{
    public int Start { get; }
    public int Length { get; }

    public PositionalFieldAttribute(int start, int length)
    {
        Start = start;
        Length = length;
    }
}
