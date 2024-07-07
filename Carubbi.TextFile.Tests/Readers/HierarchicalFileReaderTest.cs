using Carubbi.TextFile.Readers;
using Carubbi.TextFile.Configuration;
using Carubbi.TextFile.Tests.Readers.Models;

namespace Carubbi.TextFile.Tests.Readers;

public class HierarchicalFileReaderTest
{
    [Fact]
    public void GivenFileInValidFormatWithNullValuesInOptionalFields_WhenRead_ShouldParse()
    {
        // arrange
        var fileContent = """
                          P,Raphael Carubbi Neto,29/09/1981,2
                          F,John Doe,4/2/2010
                          F,Bob B.,30/05/2002
                          T,1122223344
                          T,4477776655
                          P,Armando Miani,10/10/1985,2
                          F,Joao Da Silav,11/5/2010
                          F,Jose Maria,10/11/2008
                          T,123443254
                          T,987654322
                          """;

        File.WriteAllText("test6.txt", fileContent);

        var result = HierarchicalTextFileReader.ReadHierarchicalFile<Person, Child, Phone>("test6.txt", new ReadingOptions { Mode = ContentMode.Delimited });
    }
}