using System.Buffers;

namespace Carubbi.TextFile.Readers;

internal static class CsvFieldReader
{
    public static string GetFieldAt(ReadOnlySpan<char> line, char delimiter, int fieldIndex)
    {
        if (fieldIndex < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(fieldIndex));
        }

        int currentField = 0;
        int i = 0;

        while (true)
        {
            if (currentField == fieldIndex)
                return ReadField(line, delimiter, ref i);

            // Skip field without materializing
            SkipField(line, delimiter, ref i);
            currentField++;

            if (i > line.Length) // safety
                return string.Empty;
        }
    }

    private static void SkipField(ReadOnlySpan<char> line, char delimiter, ref int i)
    {
        bool inQuotes = false;

        if (i < line.Length && line[i] == '"')
        {
            inQuotes = true;
            i++;
        }

        while (i < line.Length)
        {
            char c = line[i++];

            if (inQuotes)
            {
                if (c == '"')
                {
                    if (i < line.Length && line[i] == '"') { i++; } // escaped quote
                    else inQuotes = false; // closing quote
                }
            }
            else
            {
                if (c == delimiter) return;
            }
        }

        // end of line -> done
        i = line.Length + 1; // sentinel
    }

    private static string ReadField(ReadOnlySpan<char> line, char delimiter, ref int i)
    {
        if (i > line.Length) return string.Empty;

        bool quoted = false;
        bool needsUnescape = false;

        int start = i;

        if (i < line.Length && line[i] == '"')
        {
            quoted = true;
            i++;               // skip opening quote
            start = i;
        }

        // First pass: find end and detect if we need unescape
        int fieldStart = start;
        int fieldEndExclusive;

        if (quoted)
        {
            while (i < line.Length)
            {
                char c = line[i++];

                if (c == '"')
                {
                    if (i < line.Length && line[i] == '"')
                    {
                        needsUnescape = true;
                        i++; // consume escaped quote
                    }
                    else
                    {
                        // closing quote reached
                        fieldEndExclusive = i - 1; // exclude closing quote
                        // consume delimiter if present
                        if (i < line.Length && line[i] == delimiter) i++;
                        return needsUnescape
                            ? UnescapeQuoted(line.Slice(fieldStart, fieldEndExclusive - fieldStart))
                            : new string(line.Slice(fieldStart, fieldEndExclusive - fieldStart));
                    }
                }
            }

            throw new FormatException("Malformed CSV: missing closing quote.");
        }
        else
        {
            while (i < line.Length)
            {
                if (line[i] == delimiter)
                {
                    fieldEndExclusive = i;
                    i++; // consume delimiter
                    return new string(line.Slice(fieldStart, fieldEndExclusive - fieldStart));
                }
                i++;
            }

            fieldEndExclusive = line.Length;
            i = line.Length + 1; // sentinel
            return new string(line.Slice(fieldStart, fieldEndExclusive - fieldStart));
        }
    }

    private static string UnescapeQuoted(ReadOnlySpan<char> quotedContent)
    {
        // quotedContent is inside quotes already (no surrounding ")
        // unescape "" -> "
        int len = quotedContent.Length;
        char[] rented = ArrayPool<char>.Shared.Rent(len);
        int w = 0;

        for (int i = 0; i < len; i++)
        {
            char c = quotedContent[i];
            if (c == '"' && i + 1 < len && quotedContent[i + 1] == '"')
            {
                rented[w++] = '"';
                i++; // skip second quote
            }
            else
            {
                rented[w++] = c;
            }
        }

        var s = new string(rented, 0, w);
        ArrayPool<char>.Shared.Return(rented);
        return s;
    }
}
