using Carubbi.TextFile.FluentApi;
using Carubbi.TextFile.Tests.Writers.Models;

namespace Carubbi.TextFile.Tests.Writers.Configuration;

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
