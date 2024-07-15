using Carubbi.TextFile.Configuration;
using Carubbi.TextFile.Extensions;
using System.Collections;

namespace Carubbi.TextFile.Readers;
public class HierarchicalTextFileReader : TextFileReaderBase
{
    public static List<T1> ReadHierarchicalFile<T1, T2>(string filePath, ReadingOptions readingOptions)
        where T1 : new()
        where T2 : new()
    {
        return ReadHierarchicalFile<T1>(filePath, readingOptions, typeof(T2));
    }

    public static List<T1> ReadHierarchicalFile<T1, T2, T3>(string filePath, ReadingOptions readingOptions)
        where T1 : new()
        where T2 : new()
        where T3 : new()
    {
        return ReadHierarchicalFile<T1>(filePath, readingOptions, typeof(T2), typeof(T3));
    }

    public static List<T1> ReadHierarchicalFile<T1, T2, T3, T4>(string filePath, ReadingOptions readingOptions)
        where T1 : new()
        where T2 : new()
        where T3 : new()
        where T4 : new()
    {
        return ReadHierarchicalFile<T1>(filePath, readingOptions, typeof(T2), typeof(T3), typeof(T4));
    }

    static List<TParent> ReadHierarchicalFile<TParent>(string filePath, ReadingOptions readingOptions, params Type[] childTypes) where TParent : new()
    {
        var lines = File.ReadAllLines(filePath);
        return ReadLines<TParent>(lines, readingOptions.Mode, childTypes);
    }

    private static List<TParent> ReadLines<TParent>(string[] lines, ContentMode mode, params Type[] childTypes) where TParent : new()
    {
        var result = new List<TParent>();
        var allTypes = new[] { typeof(TParent) }.Concat(childTypes).ToArray();
        var instances = new Dictionary<Type, IList>
        {
            { typeof(TParent), new List<TParent>() }
        };

        foreach (var childType in childTypes)
        {
            instances[childType] = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(childType))!;
        }

        object? currentParent = null;
        foreach (var line in lines)
        {
            var recordType = GetRecordType(line, mode, allTypes);

            var instance = Activator.CreateInstance(recordType)!;
            var identifier = instance.GetType().GetIdentifierProperty();
            identifier?.SetValue(instance, Guid.NewGuid());

            ProcessLine(line, instance, mode);

            if (recordType == typeof(TParent))
            {
                if (currentParent != null)
                {
                    result.Add((TParent)currentParent);
                }
                currentParent = instance;
            }
            else
            {
                LinkChildren(currentParent, instance);
            }

            instances[recordType].Add(instance);
        }

        if (currentParent != null)
        {
            result.Add((TParent)currentParent);
        }

        return result;

    }


    private static Type GetRecordType(string line, ContentMode mode, Type[] allTypes)
    {
        foreach (var type in allTypes)
        {
            var expectedRecordType = type.GetRecordType() ?? throw new InvalidOperationException($"RecordTypeAttribute is required for type {type.Name}.");
            
            var lineRecordType = string.Empty;
            
            if (mode == ContentMode.Delimited)
            {
                var delimiterField = type.GetDelimiter() ?? throw new InvalidOperationException("Delimiter configuration is required for delimited mode.");
                var recordTypeOrder = type.GetFieldOrder() ?? throw new InvalidOperationException("Record type order is required for delimited mode"); 
                lineRecordType = line.Split(delimiterField)[recordTypeOrder - 1];
            }
            else
            {
                var recordTypeStartIndex = type.GetStartIndex() ?? throw new InvalidOperationException("Record Type Start Index is required for positional mode."); 
                var recordTypeLength = type.GetFieldLength() ?? throw new InvalidOperationException("Record Type length is required for positional mode."); 
                 
                lineRecordType = line.Substring(recordTypeStartIndex, recordTypeLength);
            }

            if (lineRecordType == expectedRecordType)
            {
                return type;
            }
        }

        throw new InvalidOperationException("Record type on line doesn't match with any Record layout in the hierarchy");
    }

    private static void UpdateForeignKey(object parent, object child)
    {
        var parentReferenceField = child.GetType().GetParentReferenceProperty(parent.GetType());
        var recordIdentifierField = parent.GetType().GetIdentifierProperty();

        if (parentReferenceField != null && recordIdentifierField != null)
        {
            parentReferenceField.SetValue(child, recordIdentifierField.GetValue(parent));
        }
    }

    private static void LinkChildren(object parent, object child)
    {
        var childReferenceField = parent.GetType().GetChildReferenceCollection(child.GetType());

        if (childReferenceField != null)
        {
            var childList = childReferenceField.GetValue(parent) as IList;
            if (childList == null)
            {
                childList = Activator.CreateInstance(typeof(List<>).MakeGenericType(child.GetType())) as IList;
                childReferenceField.SetValue(parent, childList);
            }

            childList!.Add(child);
        }

        UpdateForeignKey(parent, child);
    }
}
