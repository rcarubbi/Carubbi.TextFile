namespace Carubbi.TextFile.Tests.Readers.Models;

public class FluentPerson
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public DateTime? Dob { get; set; }

    public int? ChildrenCount { get; set; }

    public List<Child> Children { get; set; } = new();

    public List<Phone> Phones { get; set; } = new();
}