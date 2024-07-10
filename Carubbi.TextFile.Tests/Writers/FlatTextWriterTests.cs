using Carubbi.TextFile.Attributes;
using Carubbi.TextFile.Configuration;
using Carubbi.TextFile.Writers;
using FluentAssertions;

namespace Carubbi.TextFile.Tests.Writers;

public class FlatTextWriterTests
{
    [Fact]
    public async Task GivenModelsWithDelimitedAttributes_WhenWriterIsCalled_ThenShouldCreateTheExpectedLayout()
    {
        var models = new List<WriterRecord>() {
            new() {
                Name = "Joao da Silva",
                Dob = new DateTime(2020, 10, 11),
                ChildrenCount = 2
            },
            new() {
                Name = "Jose dos Santos",
                Dob = new DateTime(2005, 4, 11),
                ChildrenCount = 5
            },
        };

        var expectedContent = """
                              Name,Dob,ChildrenCount
                              Joao da Silva,11/10/2020,2
                              Jose dos Santos,11/04/2005,5

                              """;

        await FlatTextFileWriter.WriteFileAsync(models, "file1.csv", new WritingOptions { Header = "Name,Dob,ChildrenCount", Mode = ContentMode.Delimited }, CancellationToken.None);

        var content = File.ReadAllText("file1.csv");
        content.Should().Be(expectedContent);
    }

    [Fact]
    public async Task GivenModelsWithDelimitedAttributes_WhenWriterCalledWithIgnoreLastLineBreak_ThenShouldCreateTheExpectedLayout()
    {
        var models = new List<WriterRecord>() {
            new() {
                Name = "Joao da Silva",
                Dob = new DateTime(2020, 10, 11),
                ChildrenCount = 2
            },
            new() {
                Name = "Jose dos Santos",
                Dob = new DateTime(2005, 4, 11),
                ChildrenCount = 5
            },
        };

        var expectedContent = """
                              Name,Dob,ChildrenCount
                              Joao da Silva,11/10/2020,2
                              Jose dos Santos,11/04/2005,5
                              """;

        await FlatTextFileWriter.WriteFileAsync(models, "file1.csv", new WritingOptions { Header = "Name,Dob,ChildrenCount", Mode = ContentMode.Delimited, IgnoreLastLineBreak = true }, CancellationToken.None);

        var content = File.ReadAllText("file1.csv");
        content.Should().Be(expectedContent);
    }

    [Fact]
    public async Task GivenModelsWithPositionalAttributes_WhenWriterCalled_ThenShouldCreateTheExpectedLayout()
    {
        var models = new List<WriterRecord>() {
            new() {
                Name = "Joao da Silva",
                Dob = new DateTime(2020, 10, 11),
                ChildrenCount = 2
            },
            new() {
                Name = "Jose dos Santos",
                Dob = new DateTime(2005, 4, 11),
                ChildrenCount = 5
            },
        };

        var expectedContent = """
                              Name,Dob,ChildrenCount
                              Joao da Silva       11/10/202002
                              Jose dos Santos     11/04/200505

                              """;

        await FlatTextFileWriter.WriteFileAsync(models, "file1.csv", new WritingOptions { Header = "Name,Dob,ChildrenCount", Mode = ContentMode.Positional  }, CancellationToken.None);

        var content = File.ReadAllText("file1.csv");
        content.Should().Be(expectedContent);
    }
}

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