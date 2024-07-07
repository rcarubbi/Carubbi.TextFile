using System.Reflection;

namespace Carubbi.TextFile.FluentApi;

public class ParentConfig
{
    public Type ParentType { get; set; }

    public PropertyInfo Property { get; set; }
}

