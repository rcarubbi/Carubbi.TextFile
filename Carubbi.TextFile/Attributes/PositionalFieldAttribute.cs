using Carubbi.TextFile.Configuration;

namespace Carubbi.TextFile.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
public class PositionalFieldAttribute : Attribute
{
    public int Start { get; }
    public int Length { get; }

    public PaddingDirection PaddingDirection { get; }

    public char PaddingChar { get; }

    public PositionalFieldAttribute(int start, int length, char paddingChar = '\0', PaddingDirection paddingDirection = PaddingDirection.NotSet)
    {
        Start = start;
        Length = length;
        PaddingDirection = paddingDirection;
        PaddingChar = paddingChar;
    }
}
 
