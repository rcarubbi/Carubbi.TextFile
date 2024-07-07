namespace Carubbi.TextFile.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class RecordTypeAttribute : Attribute
{
    public string RecordType { get; }

    public RecordTypeAttribute(string recordType)
    {
        RecordType = recordType;

    }
}
