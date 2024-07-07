using Carubbi.TextFile.Tests.Readers.Models;
using Carubbi.TextFile.FluentApi;

namespace Carubbi.TextFile.Tests.Readers.Configuration;

public class FluentPersonConfiguration : TextFileRecordTypeConfiguration<FluentPerson>
{
    public FluentPersonConfiguration()
    {
        HasDelimiter(',');
        HasRecordType("P").InDelimitedOrder(1);
        Property(p => p.Id).AsIdentifier();
        Property(p => p.Name).InDelimitedOrder(2);
        Property(p => p.Dob).InDelimitedOrder(3);
        Property(p => p.ChildrenCount).InDelimitedOrder(4);

        HasChild(p => p.Children)
            .WithParentReference(x => x.ParentId); // option to reference a parent from itself

        HasChild(p => p.Phones)
            .WithParentReference(x => x.ParentId);
    }
}