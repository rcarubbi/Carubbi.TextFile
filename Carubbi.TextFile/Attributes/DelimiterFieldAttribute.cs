namespace Carubbi.TextFile.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
public class DelimiterFieldAttribute : Attribute
{
    public int Order { get; }
 
    public DelimiterFieldAttribute(int order)
    {
        Order = order;
    }
}
