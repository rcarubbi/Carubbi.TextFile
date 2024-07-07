using Carubbi.TextFile.Attributes;

namespace Carubbi.TextFile.Tests.Readers.Models;

[Delimiter(',')]
[RecordType("T")]
[DelimiterField(1)]
public class Phone
{
    [DelimiterField(2)]
    public string Number { get; set; } = null!;

    [ParentReferenceField<Person>]
    public Guid ParentId { get; set; }
}