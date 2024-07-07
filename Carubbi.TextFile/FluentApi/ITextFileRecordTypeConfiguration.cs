namespace Carubbi.TextFile.FluentApi;

internal interface ITextFileRecordTypeConfiguration
{
    string? RecordType { get; }

    char? Delimiter { get; }

    int? RecordTypeOrder { get; }
    int? RecordTypeStartIndex { get; }
    int? RecordTypeLength { get; }

    Dictionary<string, FieldConfig> Fields { get; }

    List<ChildConfig> ChildConfigs { get; }

    List<ParentConfig> ParentConfigs { get; }

}

