using Carubbi.TextFile.Tests.Readers.Models;
using Carubbi.TextFile.FluentApi;

namespace Carubbi.TextFile.Tests.Readers.Configuration;

public class FluentChildConfiguration : TextFileRecordTypeConfiguration<FluentChild>
{
    public FluentChildConfiguration()
    {
        HasDelimiter(',');
        HasRecordType("F").InDelimitedOrder(1);
        Property(p => p.Name).InDelimitedOrder(2);
        Property(p => p.Dob).InDelimitedOrder(3);
        Property(p => p.ParentId)
            .AsParentReference<Person>(); // option to reference a parent from child (one is enough)
    }
}