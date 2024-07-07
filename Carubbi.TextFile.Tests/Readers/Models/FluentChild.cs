namespace Carubbi.TextFile.Tests.Readers.Models;

public class FluentChild
{
    public string Name { get; set; } = null!;

    public DateTime? Dob { get; set; }

    public Guid ParentId { get; set; }
}