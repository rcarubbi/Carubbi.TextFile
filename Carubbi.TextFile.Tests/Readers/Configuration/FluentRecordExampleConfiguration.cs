using Carubbi.TextFile.Tests.Readers.Models;
using Carubbi.TextFile.FluentApi;

namespace Carubbi.TextFile.Tests.Readers.Configuration;

public class FluentRecordExampleConfiguration : TextFileRecordTypeConfiguration<FluentRecordExample>
{
    public FluentRecordExampleConfiguration()
    {
        HasDelimiter(',');
        Property(x => x.Name).InDelimitedOrder(1).InPositionalIndex(0, 20);
        Property(x => x.DateOfBirth).InDelimitedOrder(2).InPositionalIndex(20, 10);
        Property(x => x.Children).InDelimitedOrder(3).InPositionalIndex(30, 2);
    }
}
