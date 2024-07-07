using System.Linq.Expressions;
using System.Reflection;

namespace Carubbi.TextFile.FluentApi;

public class ChildBuilder<TChild, TParent> where TParent : class
{
    private readonly List<ParentConfig> _parentConfigs;

    internal ChildBuilder(List<ParentConfig> parentConfigs)
    {
        _parentConfigs = parentConfigs;
    }

    public void WithParentReference(Expression<Func<TChild, Guid>> propertyExpression)
    {
        var property = (PropertyInfo)((MemberExpression)propertyExpression.Body).Member;
        _parentConfigs.Add(new ParentConfig
        {
            ParentType = typeof(TParent),
            Property = property,
        });
    }
}

