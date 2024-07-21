using Carubbi.TextFile.Attributes;
using Carubbi.TextFile.Configuration;

namespace Carubbi.TextFile.Tests.Writers.Models;

[Delimiter(',')]
public class WriterRecord
{
    [DelimiterField(1)]
    [PositionalField(0, 20, ' ', PaddingDirection.Right)]
    public string Name { get; set; }

    [DelimiterField(2)]
    [WriteCustomParse(nameof(FormatDate))]
    [PositionalField(20, 10, ' ', PaddingDirection.Right)]
    public DateTime Dob { get; set; }

    [DelimiterField(3)]
    [PositionalField(30, 2, '0', PaddingDirection.Left)]
    public int? ChildrenCount { get; set; }

    private string FormatDate(DateTime date)
    {
        return date.ToShortDateString();
    }
}
