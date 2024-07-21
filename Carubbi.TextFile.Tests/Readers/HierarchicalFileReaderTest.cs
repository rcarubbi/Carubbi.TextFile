using Carubbi.TextFile.Readers;
using Carubbi.TextFile.Configuration;
using Carubbi.TextFile.Tests.Readers.Models;
using FluentAssertions;

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
                          F,Joao Da Silva,11/5/2010
                          F,Jose Maria,10/11/2008
                          T,123443254
                          T,987654322
                          """;

        File.WriteAllText("test6.txt", fileContent);

        var reader = new HierarchicalTextFileReader(new ReadingOptions { Mode = ContentMode.Delimited });

        // act
        var result = reader.ReadHierarchicalFile<Person, Child, Phone>("test6.txt");

        // assert
        result.Should().HaveCount(2);
        result[0].Name.Should().Be("Raphael Carubbi Neto");
        result[0].Dob.Should().Be(new DateTime(1981, 9, 29));
        result[0].ChildrenCount.Should().Be(2);
        result[0].Children.Should().HaveCount(2);
        result[0].Children[0].Name.Should().Be("John Doe");
        result[0].Children[0].Dob.Should().Be(new DateTime(2010, 2, 4));
        result[0].Children[1].Name.Should().Be("Bob B.");
        result[0].Children[1].Dob.Should().Be(new DateTime(2002, 5, 30));
        result[0].Phones.Should().HaveCount(2);
        result[0].Phones[0].Number.Should().Be("1122223344");
        result[0].Phones[1].Number.Should().Be("4477776655");

        result[1].Name.Should().Be("Armando Miani");
        result[1].Dob.Should().Be(new DateTime(1985, 10, 10));
        result[1].ChildrenCount.Should().Be(2);
        result[1].Children.Should().HaveCount(2);
        result[1].Children[0].Name.Should().Be("Joao Da Silva");
        result[1].Children[0].Dob.Should().Be(new DateTime(2010, 5, 11));
        result[1].Children[1].Name.Should().Be("Jose Maria");
        result[1].Children[1].Dob.Should().Be(new DateTime(2008, 11, 10));
        result[1].Phones.Should().HaveCount(2);
        result[1].Phones[0].Number.Should().Be("123443254");
        result[1].Phones[1].Number.Should().Be("987654322");
    }
}