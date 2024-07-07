using Carubbi.TextFile.Tests.Readers.Models;
using Carubbi.TextFile.FluentApi;

namespace Carubbi.TextFile.Tests.Readers.Configuration;

public class FluentPhoneConfiguration : TextFileRecordTypeConfiguration<FluentPhone>
{
    public FluentPhoneConfiguration()
    {
        HasDelimiter(',');
        HasRecordType("T").InDelimitedOrder(1);
        Property(p => p.Number).InDelimitedOrder(2);
        Property(p => p.ParentId)
            .AsParentReference<Person>(); // option to reference a parent from child (one is enough)
    }
}