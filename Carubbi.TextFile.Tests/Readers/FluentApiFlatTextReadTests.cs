using Carubbi.TextFile.Readers;
using Carubbi.TextFile.Tests.Readers.Configuration;
using Carubbi.TextFile.Tests.Readers.Models;
using Carubbi.TextFile.Configuration;
using Carubbi.TextFile.FluentApi;

namespace Carubbi.TextFile.Tests.Readers;

public class FluentApiFlatTextReadTests
{
    public FluentApiFlatTextReadTests()
    {
        TextFileModelBuilder.ApplyConfigurationsFromAssembly(typeof(FluentRecordExampleConfiguration).Assembly);
    }

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

        File.WriteAllText("test3.txt", fileContent);

        // sut
        var result = FlatTextFileReader.ReadFile<FluentRecordExample>("test3.txt", new ReadingOptions { SkipHeader = true, Mode = ContentMode.Delimited });

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

        File.WriteAllText("test4.txt", fileContent);

        // sut
        var result = FlatTextFileReader.ReadFile<FluentRecordExample>("test4.txt", new ReadingOptions { SkipHeader = true, Mode = ContentMode.Positional });

    }
}
