using Carubbi.TextFile.Attributes;
using System.Reflection;
using static Carubbi.TextFile.FluentApi.TextFileModelBuilder;

namespace Carubbi.TextFile.Extensions;

internal static class TypeExtensions
{
    internal static string? GetRecordType(this Type type)
    {
        if (type.GetCustomAttribute(typeof(RecordTypeAttribute)) is RecordTypeAttribute recordTypeAttribute)
        {
            return recordTypeAttribute?.RecordType;
        }
        else
        {
            var hasConfiguration = Configs.TryGetValue(type, out var configuration);
            return hasConfiguration
                ? configuration!.RecordType 
                : null;
        }
    }

    internal static char? GetDelimiter(this Type type)
    {
        if (type.GetCustomAttribute(typeof(DelimiterAttribute)) is DelimiterAttribute delimiterAttribute)
        {
            return delimiterAttribute.Delimiter;
        }
        else
        {
            var hasConfiguration = Configs.TryGetValue(type, out var configuration);
            
            return hasConfiguration
                ? configuration!.Delimiter 
                : null;
        }
    }

    internal static int? GetFieldLength(this Type type)
    {
        if (type.GetCustomAttribute(typeof(PositionalFieldAttribute)) is PositionalFieldAttribute positionalFieldAttribute)
        {
            return positionalFieldAttribute.Length;
        }
        else
        {
            var hasConfiguration = Configs.TryGetValue(type, out var configuration);
            return hasConfiguration 
                ? configuration!.RecordTypeLength 
                : null;
        }
    }

    internal static int? GetStartIndex(this Type type)
    {
        if (type.GetCustomAttribute(typeof(PositionalFieldAttribute)) is PositionalFieldAttribute positionalFieldAttribute)
        {
            return positionalFieldAttribute.Start;
        }
        else
        {
            var hasConfiguration = Configs.TryGetValue(type, out var configuration);
            return hasConfiguration 
                ? configuration!.RecordTypeStartIndex
                : null;
        }
    }

    internal static int? GetFieldOrder(this Type type)
    {
        if (type.GetCustomAttribute(typeof(DelimiterFieldAttribute)) is DelimiterFieldAttribute delimiterFieldAttribute)
        {
            return delimiterFieldAttribute.Order;
        }
        else
        {
            var hasConfiguration = Configs.TryGetValue(type, out var configuration);
            return hasConfiguration 
                ? configuration!.RecordTypeOrder 
                : null;
        }
    }

    internal static PropertyInfo? GetIdentifierProperty(this Type type)
    {
        var identifierProperty = type.GetProperties()
         .Where(p => p.GetCustomAttributes(typeof(RecordIdentifierAttribute), false).Any())
         .SingleOrDefault();

        if (identifierProperty == null)
        {
            var hasConfiguration = Configs.TryGetValue(type, out var configuration);

            return (hasConfiguration
                ? configuration!.Fields.FirstOrDefault(x => x.Value.IsIdentifier).Value?.Property
                : null);
        }
        else
        {
            return identifierProperty;
        }
    }

    internal static PropertyInfo? GetParentReferenceProperty(this Type type, Type parentType)
    {
        var parentReferenceFieldAttributeType = typeof(ParentReferenceFieldAttribute<>).MakeGenericType(parentType);
        var parentReference = type.GetProperties()
        .Where(p => p.GetCustomAttributes(parentReferenceFieldAttributeType, false).Any())
        .SingleOrDefault();

        if (parentReference == null)
        {
            var hasParentConfiguration = Configs.TryGetValue(parentType, out var parentConfig);
            var hasChildConfiguration = Configs.TryGetValue(type, out var childConfig);

            return (hasParentConfiguration
                ? parentConfig!.ParentConfigs.FirstOrDefault(x => x.ParentType == parentType)?.Property
                : hasChildConfiguration
                ? childConfig!.Fields.FirstOrDefault(x => x.Value.ParentReferenceType == parentType).Value?.Property
                : null);
        }
        else
        {
            return parentReference;
        }

    }

    internal static PropertyInfo? GetChildReferenceCollection(this Type type, Type childType)
    {
        var childReferenceProperty = type.GetProperties()
       .Where(p => p.GetCustomAttributes(typeof(ChildReferenceCollectionAttribute), false).Any() && p.GetListItemType() == childType)
       .SingleOrDefault();

        if (childReferenceProperty == null)
        {
            var hasConfiguration = Configs.TryGetValue(type, out var configuration);
            return (hasConfiguration
                    ? configuration!.ChildConfigs.FirstOrDefault(x => x.ChildType == childType)?.Property
                    : null);
        }
        else
        {
            return childReferenceProperty;
        }
    }

    internal static PropertyInfo[] GetDelimitedProperties(this Type type)
    {
        var propertiesFromAttributes = type.GetProperties().Where(p => p.GetCustomAttributes(typeof(DelimiterFieldAttribute), false).Any())
            .OrderBy(x => x.GetCustomAttribute<DelimiterFieldAttribute>()!.Order);

        if (propertiesFromAttributes.Any())
        {
            return propertiesFromAttributes.ToArray();
        }
        else
        {
            var hasConfiguration = Configs.TryGetValue(type, out var configuration);
            return hasConfiguration
                ? configuration!.Fields
                    .Where(x => x.Value.Order.HasValue)
                    .OrderBy(x => x.Value.Order)
                    .Select(x => x.Value.Property!)
                    .ToArray() 
                : Array.Empty<PropertyInfo>();   
        }
    }

    internal static PropertyInfo[] GetPositionalProperties(this Type type)
    {
        var propertiesFromAttributes = type.GetProperties().Where(p => p.GetCustomAttributes(typeof(PositionalFieldAttribute), false).Any())
            .OrderBy(x => x.GetCustomAttribute<PositionalFieldAttribute>()!.Start);

        if (propertiesFromAttributes.Any())
        {
            return propertiesFromAttributes.ToArray();
        }
        else
        {
            var hasConfiguration = Configs.TryGetValue(type, out var configuration);
            return hasConfiguration
                ? configuration!.Fields
                    .Where(x => x.Value.StartIndex.HasValue && x.Value.Length.HasValue)
                    .OrderBy(x => x.Value.StartIndex)
                    .Select(x => x.Value.Property!)
                    .ToArray()
                : Array.Empty<PropertyInfo>();
        }
    }
}
