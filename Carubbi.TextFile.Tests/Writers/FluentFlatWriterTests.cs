using Carubbi.TextFile.Configuration;
using Carubbi.TextFile.FluentApi;
using Carubbi.TextFile.Writers;
using FluentAssertions;

namespace Carubbi.TextFile.Tests.Writers;

public class FluentFlatWriterTests
{
    [Fact]
    public async Task GivenModelsWithDelimitedFluentConfig_WhenWriterIsCalled_ThenShouldCreateTheExpectedLayout()
    {
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

        await FlatTextFileWriter.WriteFileAsync(models, "file2.csv", new WritingOptions { Header = "Name,Dob,ChildrenCount", Mode = ContentMode.Delimited }, CancellationToken.None);

        var content = File.ReadAllText("file2.csv");
        content.Should().Be(expectedContent);
    }


    public class FluentWriterRecordConfig : TextFileRecordTypeConfiguration<FluentWriterRecord>
    {
        public FluentWriterRecordConfig()
        {
            HasDelimiter(',');
            Property(x => x.Name).InDelimitedOrder(1).InPositionalIndex(0, 20).PadRightWith(' ');
            Property(x => x.Dob).InDelimitedOrder(2).InPositionalIndex(20, 10).PadRightWith(' ').WithWriteCustomParse<DateTime>(x => x.ToShortDateString());
            Property(x => x.ChildrenCount).InDelimitedOrder(3).InPositionalIndex(30, 2).PadLeftWith('0');
        }
    }

    public class FluentWriterRecord
    {
        public string Name { get; set; }

        public DateTime Dob { get; set; }

        public int? ChildrenCount { get; set; }
    }
} 
