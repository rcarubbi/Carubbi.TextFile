using Carubbi.TextFile.Readers;
using Carubbi.TextFile.Tests.Readers.Models;
using Carubbi.TextFile.Configuration;
using FluentAssertions;

namespace Carubbi.TextFile.Tests.Readers;

public class FlatTextReaderTests
{
    [Fact]
    public void GivenValidDelimitedFileWithNullValuesInOptionalFields_WhenRead_ShouldParse()
    {
        // arrange
        var fileContent = """
                          Name,DOB,Children
                          Raphael Carubbi Neto,29/09/1981,2
                          John Doe,,1
                          Bob B.,30/05/2002
                          """;

        File.WriteAllText("test1.txt", fileContent);

        // sut
        var result = FlatTextFileReader.ReadFile<RecordExample>("test1.txt", new ReadingOptions { SkipHeader = true, Mode = ContentMode.Delimited });

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
    public void GivenValidPositionalFileWithEmptyValuesInOptionalFields_WhenRead_ShouldParse()
    {
        // arrange
        var fileContent = """
                          Name (20) DOB (10) Children (2)
                          Raphael Carubbi Neto29/09/198102
                          John Doe                      01
                          Bob B.              30/05/2002  
                          """;

        File.WriteAllText("test2.txt", fileContent);

        // sut
        var result = FlatTextFileReader.ReadFile<RecordExample>("test2.txt", new ReadingOptions { SkipHeader = true, Mode = ContentMode.Positional });

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