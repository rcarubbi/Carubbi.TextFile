using Carubbi.TextFile.Configuration;
using Carubbi.TextFile.FluentApi;
using Carubbi.TextFile.Tests.Writers.Models;
using Carubbi.TextFile.Writers;
using FluentAssertions;

namespace Carubbi.TextFile.Tests.Writers;

public class FluentFlatWriterTests
{
    [Fact]
    public async Task GivenModelsWithDelimitedFluentConfig_WhenWriterIsCalled_ThenShouldCreateTheExpectedLayout()
    {
        // arrange
        TextFileModelBuilder.ApplyConfigurationsFromAssembly(typeof(FluentFlatWriterTests).Assembly);
        var models = new List<FluentWriterRecord>() {
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

        var writer = new FlatTextFileWriter<FluentWriterRecord>(new WritingOptions { Header = "Name,Dob,ChildrenCount", Mode = ContentMode.Delimited });

        // act
        await writer.WriteFileAsync(models, "file2.csv", CancellationToken.None);

        // assert
        var content = File.ReadAllText("file2.csv");
        content.Should().Be(expectedContent);
    }
} 
