using Carubbi.TextFile.Readers;
using System;
using Xunit;

namespace Carubbi.TextFile.Tests.Readers;

public class CsvFieldReaderTests
{
    [Fact]
    public void GetFieldAt_Unquoted_SplitsByDelimiter()
    {
        ReadOnlySpan<char> line = "a,b,c".AsSpan();

        Assert.Equal("a", CsvFieldReader.GetFieldAt(line, ',', 0));
        Assert.Equal("b", CsvFieldReader.GetFieldAt(line, ',', 1));
        Assert.Equal("c", CsvFieldReader.GetFieldAt(line, ',', 2));
    }

    [Fact]
    public void GetFieldAt_QuotedField_CanContainDelimiters()
    {
        ReadOnlySpan<char> line = "a,b,\"c,d,e\",f".AsSpan();

        Assert.Equal("a", CsvFieldReader.GetFieldAt(line, ',', 0));
        Assert.Equal("b", CsvFieldReader.GetFieldAt(line, ',', 1));
        Assert.Equal("c,d,e", CsvFieldReader.GetFieldAt(line, ',', 2));
        Assert.Equal("f", CsvFieldReader.GetFieldAt(line, ',', 3));
    }

    [Fact]
    public void GetFieldAt_QuotedField_AllowsEscapedQuotes()
    {
        ReadOnlySpan<char> line = "a,b,\"c,\"\"d\"\",e\",f".AsSpan();

        Assert.Equal("a", CsvFieldReader.GetFieldAt(line, ',', 0));
        Assert.Equal("b", CsvFieldReader.GetFieldAt(line, ',', 1));
        Assert.Equal("c,\"d\",e", CsvFieldReader.GetFieldAt(line, ',', 2));
        Assert.Equal("f", CsvFieldReader.GetFieldAt(line, ',', 3));
    }

    [Fact]
    public void GetFieldAt_EmptyFields_AreReturnedAsEmptyString()
    {
        ReadOnlySpan<char> line = "a,,c,".AsSpan();

        Assert.Equal("a", CsvFieldReader.GetFieldAt(line, ',', 0));
        Assert.Equal("", CsvFieldReader.GetFieldAt(line, ',', 1));
        Assert.Equal("c", CsvFieldReader.GetFieldAt(line, ',', 2));
        Assert.Equal("", CsvFieldReader.GetFieldAt(line, ',', 3));
    }

    [Fact]
    public void GetFieldAt_TrailingDelimiter_ProducesFinalEmptyField()
    {
        ReadOnlySpan<char> line = "a,b,".AsSpan();

        Assert.Equal("a", CsvFieldReader.GetFieldAt(line, ',', 0));
        Assert.Equal("b", CsvFieldReader.GetFieldAt(line, ',', 1));
        Assert.Equal("", CsvFieldReader.GetFieldAt(line, ',', 2));
    }

    [Fact]
    public void GetFieldAt_QuotedEmptyField_ReturnsEmptyString()
    {
        ReadOnlySpan<char> line = "a,\"\",b".AsSpan();

        Assert.Equal("a", CsvFieldReader.GetFieldAt(line, ',', 0));
        Assert.Equal("", CsvFieldReader.GetFieldAt(line, ',', 1));
        Assert.Equal("b", CsvFieldReader.GetFieldAt(line, ',', 2));
    }

    [Fact]
    public void GetFieldAt_QuotedField_AtEndOfLine()
    {
        ReadOnlySpan<char> line = "a,b,\"c,d\"".AsSpan();

        Assert.Equal("a", CsvFieldReader.GetFieldAt(line, ',', 0));
        Assert.Equal("b", CsvFieldReader.GetFieldAt(line, ',', 1));
        Assert.Equal("c,d", CsvFieldReader.GetFieldAt(line, ',', 2));
    }

    [Fact]
    public void GetFieldAt_FieldIndexNegative_Throws()
    {
        ReadOnlySpan<char> line = "a,b,c".AsSpan();

        try
        {
            _ = CsvFieldReader.GetFieldAt(line, ',', -1);
            Assert.Fail("Expected ArgumentOutOfRangeException, but no exception was thrown.");
        }
        catch (ArgumentOutOfRangeException ex)
        {
            Assert.Equal("fieldIndex", ex.ParamName);
        }
    }


    [Fact]
    public void GetFieldAt_MissingClosingQuote_ThrowsFormatException()
    {
        ReadOnlySpan<char> line = "a,\"b,c".AsSpan();

        try
        {
            _ = CsvFieldReader.GetFieldAt(line, ',', 1);
            Assert.Fail("Expected FormatException, but no exception was thrown.");
        }
        catch (FormatException)
        {
            // ok
        }
    }

    [Fact]
    public void GetFieldAt_QuotedField_WithEscapedQuoteOnly()
    {
        ReadOnlySpan<char> line = "\"\"\"\"".AsSpan(); // inside quotes: "" => one quote

        Assert.Equal("\"", CsvFieldReader.GetFieldAt(line, ',', 0));
    }

    [Fact]
    public void GetFieldAt_EmptyLine_ReturnsSingleEmptyField()
    {
        ReadOnlySpan<char> line = "".AsSpan();

        Assert.Equal("", CsvFieldReader.GetFieldAt(line, ',', 0));
    }

    [Fact]
    public void GetFieldAt_OnlyDelimiters_ReturnsEmptyFields()
    {
        ReadOnlySpan<char> line = ",,".AsSpan();

        Assert.Equal("", CsvFieldReader.GetFieldAt(line, ',', 0));
        Assert.Equal("", CsvFieldReader.GetFieldAt(line, ',', 1));
        Assert.Equal("", CsvFieldReader.GetFieldAt(line, ',', 2));
    }
}
