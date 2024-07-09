using Carubbi.TextFile.Attributes;
using Carubbi.TextFile.Configuration;
using System.Reflection;
using static Carubbi.TextFile.FluentApi.TextFileModelBuilder;

namespace Carubbi.TextFile.Extensions;

internal static class PropertyInfoExtensions
{
    internal static bool IsNullable(this PropertyInfo propertyInfo)
    {
        var propertyType = propertyInfo.PropertyType;
        return propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>);
    }

    internal static bool IsList(this PropertyInfo propertyInfo)
    {
        var propertyType = propertyInfo.PropertyType;
        return propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(List<>);
    }

    internal static Type? GetListItemType(this PropertyInfo propertyInfo)
    {
        return propertyInfo.IsList()
          ? propertyInfo.PropertyType.GenericTypeArguments[0]
          : null;
    }

    internal static int? GetFieldLength(this PropertyInfo propertyInfo)
    {
        if (propertyInfo.GetCustomAttribute(typeof(PositionalFieldAttribute)) is PositionalFieldAttribute positionalFieldAttribute)
        {
            return positionalFieldAttribute.Length;
        }
        else
        {
            var hasConfiguration = Configs.TryGetValue(propertyInfo.DeclaringType!, out var configuration);

            return hasConfiguration 
                ? configuration!.Fields[propertyInfo.Name].Length 
                : null;
        }
    }

    internal static PaddingDirection? GetPaddingDirection(this PropertyInfo propertyInfo)
    {
        if (propertyInfo.GetCustomAttribute(typeof(PositionalFieldAttribute)) is PositionalFieldAttribute positionalFieldAttribute)
        {
            return positionalFieldAttribute.PaddingDirection;
        }
        else
        {
            var hasConfiguration = Configs.TryGetValue(propertyInfo.DeclaringType!, out var configuration);

            return hasConfiguration
                ? configuration!.Fields[propertyInfo.Name].PaddingDirection  
                : null;
        }
    }

    internal static char? GetPaddingChar(this PropertyInfo propertyInfo)
    {
        if (propertyInfo.GetCustomAttribute(typeof(PositionalFieldAttribute)) is PositionalFieldAttribute positionalFieldAttribute)
        {
            return positionalFieldAttribute.PaddingChar;
        }
        else
        {
            var hasConfiguration = Configs.TryGetValue(propertyInfo.DeclaringType!, out var configuration);

            return hasConfiguration
                ? configuration!.Fields[propertyInfo.Name].PaddingChar
                : null;
        }
    }



    internal static int? GetStartIndex(this PropertyInfo propertyInfo)
    {
        if (propertyInfo.GetCustomAttribute(typeof(PositionalFieldAttribute)) is PositionalFieldAttribute positionalFieldAttribute)
        {
            return positionalFieldAttribute.Start;
        }
        else
        {
            var hasConfiguration = Configs.TryGetValue(propertyInfo.DeclaringType!, out var configuration);

            return hasConfiguration
                ? configuration!.Fields[propertyInfo.Name].StartIndex
                : null;
        }
    }

    internal static int? GetFieldOrder(this PropertyInfo propertyInfo)
    {
        if (propertyInfo.GetCustomAttribute(typeof(DelimiterFieldAttribute)) is DelimiterFieldAttribute delimiterFieldAttribute)
        {
            return delimiterFieldAttribute.Order;
        }
        else
        {
            var hasConfiguration = Configs.TryGetValue(propertyInfo.DeclaringType!, out var configuration);

            return hasConfiguration
                ? configuration!.Fields[propertyInfo.Name].Order
                : null;
        }
    }

    internal static Type GetPropertyType(this PropertyInfo propertyInfo)
    {
        return propertyInfo.IsNullable()
            ? propertyInfo.PropertyType.GenericTypeArguments[0]
            : propertyInfo.PropertyType;
    }

    internal static MethodInfo? GetCustomParseMethod(this PropertyInfo propertyInfo)
    {
        if (propertyInfo.GetCustomAttribute(typeof(WriteCustomParseAttribute)) is WriteCustomParseAttribute writeCustomParseAttribute)
        {
            return propertyInfo.DeclaringType!.GetMethod(writeCustomParseAttribute.MethodName, BindingFlags.NonPublic | BindingFlags.Instance) 
                ?? propertyInfo.DeclaringType!.GetMethod(writeCustomParseAttribute.MethodName, BindingFlags.Public | BindingFlags.Instance)
                ?? throw new InvalidOperationException($"Custom parse method {writeCustomParseAttribute.MethodName} not found in the model");
        }
        else
        {
            var hasConfiguration = Configs.TryGetValue(propertyInfo.DeclaringType!, out var configuration);

            return hasConfiguration
                ? configuration!.Fields[propertyInfo.Name].WriteCustomParse.Method
                : null;
        }
    }
}
