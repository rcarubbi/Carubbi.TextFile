﻿using Carubbi.TextFile.Configuration;

namespace Carubbi.TextFile.FluentApi;

public class FieldBuilder
{
    private FieldConfig _config;
    internal FieldBuilder(FieldConfig fieldConfig)
    {
        _config = fieldConfig;
    }

    public FieldBuilder InDelimitedOrder(int order)
    {
        _config.Order = order;
        return this;
    }

    public FieldBuilder InPositionalIndex(int startIndex, int length)
    {
        _config.StartIndex = startIndex;
        _config.Length = length;
        return this;
    }

    public void AsParentReference<T>()
    {
        _config.ParentReferenceType = typeof(T);
    }

    public FieldBuilder PadRightWith(char paddingChar)
    {
        _config.PaddingChar = paddingChar;
        _config.PaddingDirection = PaddingDirection.Right;
        return this;
    }

    public FieldBuilder PadLeftWith(char paddingChar)
    {
        _config.PaddingChar = paddingChar;
        _config.PaddingDirection = PaddingDirection.Left;
        return this;
    }

    public FieldBuilder WithWriteCustomParse<T>(Func<T, string> parse) where T : class
    {
        _config.WriteCustomParse = (Func<object, string>)parse;
        return this;
    }

    public void AsIdentifier()
    {
        _config.IsIdentifier = true;
    }
}

