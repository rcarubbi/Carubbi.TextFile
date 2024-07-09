namespace Carubbi.TextFile.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class WriteCustomParseAttribute : Attribute
{
    public string MethodName { get;  }

    public WriteCustomParseAttribute(string methodName)
    {
        MethodName = methodName;
    }
}
