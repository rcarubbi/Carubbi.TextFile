using Carubbi.TextFile.Attributes;
using Carubbi.TextFile.Configuration;
using Carubbi.TextFile.Writers;

namespace Carubbi.TextFile.Tests.Writers;

public class FlatTextWriterTests
{
    [Fact]
    public async Task GivenDecoratedCollectionWriterShouldCreateTheExpectedLayout()
    {
        var models = new List<WriterRecord>() {
            new() {
                Name = "Joao da Silva",
                Dob = new DateTime(2020, 10, 11),
                ChildrenCount = 10
            },
            new() {
                Name = "Jose dos Santos",
                Dob = new DateTime(2005, 4, 11),
                ChildrenCount = 10
            },
        };

        await FlatTextFileWriter.WriteFileAsync(models, "file1.csv", new WritingOptions { Header = "Name,Dob,ChildrenCount", Mode = ContentMode.Delimited }, CancellationToken.None);
    }
}

[Delimiter(',')]
public class WriterRecord 
{
    [DelimiterField(1)]
    public string Name { get; set; }

    [DelimiterField(2)]
    [WriteCustomParse(nameof(FormatDate))]
    public DateTime Dob { get; set; }

    [DelimiterField(3)]
    public int? ChildrenCount { get; set; }

    private string FormatDate(DateTime date)
    {
        return date.ToShortDateString();
    }
}