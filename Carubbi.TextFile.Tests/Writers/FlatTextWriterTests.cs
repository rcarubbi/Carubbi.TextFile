using Carubbi.TextFile.Configuration;
using Carubbi.TextFile.Tests.Writers.Models;
using Carubbi.TextFile.Writers;
using FluentAssertions;

namespace Carubbi.TextFile.Tests.Writers;

public class FlatTextWriterTests
{
    [Fact]
    public async Task GivenModelsWithDelimitedAttributes_WhenWriterIsCalled_ThenShouldCreateTheExpectedLayout()
    {
        // arrange
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

        var writer = new FlatTextFileWriter<WriterRecord>(new WritingOptions { Header = "Name,Dob,ChildrenCount", Mode = ContentMode.Delimited });

        // act
        await writer.WriteFileAsync(models, "file1.csv", CancellationToken.None);

        // assert
        var content = File.ReadAllText("file1.csv");
        content.Should().Be(expectedContent);
    }

    [Fact]
    public async Task GivenModelsWithDelimitedAttributes_WhenWriterCalledWithIgnoreLastLineBreak_ThenShouldCreateTheExpectedLayout()
    {
        // arrange
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

        var writer = new FlatTextFileWriter<WriterRecord>(new WritingOptions { Header = "Name,Dob,ChildrenCount", Mode = ContentMode.Delimited, IgnoreLastLineBreak = true });

        // act
        await writer.WriteFileAsync(models, "file1.csv", CancellationToken.None);

        // assert
        var content = File.ReadAllText("file1.csv");
        content.Should().Be(expectedContent);
    }

    [Fact]
    public async Task GivenModelsWithPositionalAttributes_WhenWriterCalled_ThenShouldCreateTheExpectedLayout()
    {
        // arrange
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

        var writer = new FlatTextFileWriter<WriterRecord>(new WritingOptions { Header = "Name,Dob,ChildrenCount", Mode = ContentMode.Positional });

        // act
        await writer.WriteFileAsync(models, "file1.csv", CancellationToken.None);

        // assert
        var content = File.ReadAllText("file1.csv");
        content.Should().Be(expectedContent);
    }
}

