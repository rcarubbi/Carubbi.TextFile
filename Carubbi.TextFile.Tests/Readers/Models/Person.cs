using Carubbi.TextFile.Attributes;

namespace Carubbi.TextFile.Tests.Readers.Models;

[Delimiter(',')]
[RecordType("P")]
[DelimiterField(1)]
public class Person
{
    [RecordIdentifier]
    public Guid Id { get; set; }

    [DelimiterField(2)]
    public string Name { get; set; } = null!;

    [DelimiterField(3)]
    public DateTime? Dob { get; set; }

    [DelimiterField(4)]
    public int? ChildrenCount { get; set; }

    [ChildReferenceCollection]
    public List<Child> Children { get; set; } = new();

    [ChildReferenceCollection]
    public List<Phone> Phones { get; set; } = new();
}