﻿using Carubbi.TextFile.Configuration;
using System.Reflection;

namespace Carubbi.TextFile.FluentApi;

public class FieldConfig
{
    public FieldConfig(PropertyInfo property)
    {
         Property = property;
    }
    public PropertyInfo? Property { get; }

    public int? Order { get; internal set; }
    public int? StartIndex { get; internal set; }
    public int? Length { get; internal set; }
  

    public bool IsIdentifier { get; internal set; }
    public Type ParentReferenceType { get; internal set; }

    public char? PaddingChar { get; internal set; }

    public PaddingDirection? PaddingDirection { get; internal set; }

    public Func<object, string> WriteCustomParse { get; internal set; }
}

