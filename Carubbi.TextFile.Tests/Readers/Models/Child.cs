using Carubbi.TextFile.Attributes;

namespace Carubbi.TextFile.Tests.Readers.Models;

[Delimiter(',')]
[RecordType("F")]
[DelimiterField(1)]
public class Child
{
    [DelimiterField(2)]
    public string Name { get; set; } = null!;

    [DelimiterField(3)]
    public DateTime? Dob { get; set; }

    [ParentReferenceField<Person>]
    public Guid ParentId { get; set; }
}