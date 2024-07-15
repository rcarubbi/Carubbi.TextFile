using Carubbi.TextFile.Readers;
using Carubbi.TextFile.Tests.Readers.Configuration;
using Carubbi.TextFile.Tests.Readers.Models;
using Carubbi.TextFile.Configuration;
using Carubbi.TextFile.FluentApi;
using FluentAssertions;

namespace Carubbi.TextFile.Tests.Readers;

public class FluentApiFlatTextReaderTests
{
    public FluentApiFlatTextReaderTests()
    {
        TextFileModelBuilder.ApplyConfigurationsFromAssembly(typeof(FluentRecordExampleConfiguration).Assembly);
    }

    [Fact]
    public async Task GivenValidDelimitedFileWithNullValuesInOptionalFields_WhenRead_ShouldParse()
    {
        // arrange
        var fileContent = """
                          Name,DOB,Children
                          Raphael Carubbi Neto,29/09/1981,2
                          John Doe,,1
                          Bob B.,30/05/2002
                          """;

        File.WriteAllText("test3.txt", fileContent);

        // sut
        var result = await FlatTextFileReader.ReadFile<FluentRecordExample>("test3.txt", new ReadingOptions { SkipHeader = true, Mode = ContentMode.Delimited });

        result.Should().HaveCount(3);
        result[0].Name.Should().Be("Raphael Carubbi Neto");
        result[0].DateOfBirth.Should().Be(new DateTime(1981, 9, 29));
        result[0].Children.Should().Be(2);

        result[1].Name.Should().Be("John Doe");
        result[1].DateOfBirth.Should().BeNull();
        result[1].Children.Should().Be(1);

        result[2].Name.Should().Be("Bob B.");
        result[2].DateOfBirth.Should().Be(new DateTime(2002, 5, 30));
        result[2].Children.Should().BeNull();
    }

    [Fact]
    public async Task GivenValidPositionalFileWithEmptyValuesInOptionalFields_WhenRead_ShouldParse()
    {
        // arrange
        var fileContent = """
                          Name (20) DOB (10) Children (2)
                          Raphael Carubbi Neto29/09/198102
                          John Doe                      01
                          Bob B.              30/05/2002  
                          """;

        File.WriteAllText("test4.txt", fileContent);

        // sut
        var result = await FlatTextFileReader.ReadFile<FluentRecordExample>("test4.txt", new ReadingOptions { SkipHeader = true, Mode = ContentMode.Positional });

        result.Should().HaveCount(3);
        result[0].Name.Should().Be("Raphael Carubbi Neto");
        result[0].DateOfBirth.Should().Be(new DateTime(1981, 9, 29));
        result[0].Children.Should().Be(2);

        result[1].Name.Should().Be("John Doe");
        result[1].DateOfBirth.Should().BeNull();
        result[1].Children.Should().Be(1);

        result[2].Name.Should().Be("Bob B.");
        result[2].DateOfBirth.Should().Be(new DateTime(2002, 5, 30));
        result[2].Children.Should().BeNull();
    }
}
