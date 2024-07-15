using Carubbi.TextFile.Attributes;

namespace Carubbi.TextFile.Tests.Readers.Models;

[Delimiter(',')]
public class RecordExample
{
    [DelimiterField(1)]
    [PositionalField(0, 3)]
    public int Number {  get; set; }

    [DelimiterField(2)]
    [PositionalField(3, 20)]
    public string Name { get; set; } = null!;


    [DelimiterField(3)]
    [PositionalField(23, 10)]
    public DateTime? DateOfBirth { get; set; }


    [DelimiterField(4)]
    [PositionalField(33, 2)]
    public int? Children { get; set; }
}