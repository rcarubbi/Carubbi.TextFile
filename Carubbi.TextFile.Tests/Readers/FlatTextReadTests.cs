using Carubbi.TextFile.Readers;
using Carubbi.TextFile.Tests.Readers.Models;
using Carubbi.TextFile.Configuration;

namespace Carubbi.TextFile.Tests.Readers;

public class FlatTextReadTests
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

    }
}