# Carubbi.TextFile

> Carubbi.TextFile is a text parser library designed to facilitate reading and writing text and CSV files with support for
> positional layouts, delimited-based layouts, and hierarchical layouts.
> Configuration is flexible and can be done via attributes or a fluent API.

## Features
- Positional Layouts: Define fixed-width fields in text files.
- Delimited Layouts: Define fields separated by specific delimiters, such as commas or tabs.
- Hierarchical Layouts: Handle nested or hierarchical data structures.
- Configuration Options: Use attributes or fluent API for configuration.
- Skipping Headers: Easily skip headers in files.

## Installation
Add the Carubbi.TextFile package to your project using NuGet:

```bash
dotnet add package Carubbi.TextFile
```

## Usage

### Example
Consider the following CSV content:

```csv
Name,DOB,Children
Raphael Carubbi Neto,29/09/1981,2
John Doe,,1
Bob B.,30/05/2002
```

Save this content to a file named test1.txt.

### Reading a Delimited File
To read the CSV file using Carubbi.TextFile, follow these steps:

1. Define the Record Class:

```csharp
[Delimiter(',')]
public class RecordExample
{
    [DelimiterField(1)]
    public string Name { get; set; } = null!;

    [DelimiterField(2)]
     public DateTime? DateOfBirth { get; set; }

    [DelimiterField(3)]
    public int? Children { get; set; }
}
```

2. Read the file:
```csharp
var fileContent = """
                   Name,DOB,Children
                   Raphael Carubbi Neto,29/09/1981,2
                   John Doe,,1
                   Bob B.,30/05/2002
                   """;

File.WriteAllText("test1.txt", fileContent);

var result = FlatTextFileReader.ReadFile<RecordExample>("test1.txt", new ReadingOptions { SkipHeader = true, Mode = ContentMode.Delimited });
```

## Configuration Options
- SkipHeader: Indicates whether to skip the first line (header) in the file.
- Mode: Defines the content mode (ContentMode.Delimited for CSV files or ContentMode.Positional for fixed-width files).

## Attributes
[Delimiter(char delimiter)]: Specifies the delimiter character for delimited files.
[DelimiterField(int fieldIndex)]: Indicates the position of the field in a delimited file.
[PositionalField(int startIndex, int length)]: Specifies the start index and length of the field in a positional file.

## Fluent API (Alternative Configuration)
Instead of using attributes, you can configure the parsing logic using the fluent API:
```csharp
public class FluentRecordExample
{
    public string Name { get; set; } = null!;

    public DateTime? DateOfBirth { get; set; }

    public int? Children { get; set; }
}
```

```csharp
public class FluentRecordExampleConfiguration : TextFileRecordTypeConfiguration<FluentRecordExample>
{
    public FluentRecordExampleConfiguration()
    {
        HasDelimiter(',');
        Property(x => x.Name).InDelimitedOrder(1);
        Property(x => x.DateOfBirth).InDelimitedOrder(2);
        Property(x => x.Children).InDelimitedOrder(3);
    }
}
```

Enjoy using Carubbi.TextFile for your text parsing needs!
