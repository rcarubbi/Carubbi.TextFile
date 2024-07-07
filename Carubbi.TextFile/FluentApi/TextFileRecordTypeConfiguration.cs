using System.Linq.Expressions;
using System.Reflection;

namespace Carubbi.TextFile.FluentApi;

public abstract class TextFileRecordTypeConfiguration<T> : ITextFileRecordTypeConfiguration where T : class
{
    public string? RecordType { get; private set; }
    public char? Delimiter { get; private set; }
    public int? RecordTypeOrder { get; internal set; }
    public int? RecordTypeStartIndex { get; internal set; }
    public int? RecordTypeLength { get; internal set; }

    public Dictionary<string, FieldConfig> Fields { get; } = new();
    public List<ChildConfig> ChildConfigs { get; } = new();

    public List<ParentConfig> ParentConfigs { get; } = new();
   

    protected RecordTypeBuilder<T> HasRecordType(string recordType)
    {
        RecordType = recordType;
        return new RecordTypeBuilder<T>(this);
    }

    protected void HasDelimiter(char delimiter)
    {
        Delimiter = delimiter;
    }

    protected FieldBuilder Property<TProperty>(Expression<Func<T, TProperty>> propertyExpression)
    {
        var property = (PropertyInfo)((MemberExpression)propertyExpression.Body).Member;
        Fields[property.Name] = new FieldConfig(property);
        return new FieldBuilder(Fields[property.Name]);
    }

    protected ChildBuilder<TChild, T> HasChild<TChild>(Expression<Func<T, List<TChild>>> propertyExpression)
    {
        var property = (PropertyInfo)((MemberExpression)propertyExpression.Body).Member;
        ChildConfigs.Add(new ChildConfig { Property = property, ChildType = typeof(TChild) });
        return new ChildBuilder<TChild, T>(ParentConfigs);
    }
}

