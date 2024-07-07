using Carubbi.TextFile.Attributes;

namespace Carubbi.TextFile.Tests.Readers.Models;

[Delimiter(',')]
public class RecordExample
{
    [DelimiterField(1)]
    [PositionalField(0, 20)]
    public string Name { get; set; } = null!;


    [DelimiterField(2)]
    [PositionalField(20, 10)]
    public DateTime? DateOfBirth { get; set; }


    [DelimiterField(3)]
    [PositionalField(30, 2)]
    public int? Children { get; set; }
}