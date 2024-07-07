using System.Reflection;

namespace Carubbi.TextFile.FluentApi;

public static class TextFileModelBuilder
{
    internal static readonly Dictionary<Type, ITextFileRecordTypeConfiguration> Configs = new();
    private static readonly Type ConfigType = typeof(TextFileRecordTypeConfiguration<>);

    public static void ApplyConfigurationsFromAssembly(Assembly assembly)
    {
        var configs = assembly.GetTypes()
             .Where(t => !t.IsAbstract && t.BaseType != null && t.BaseType.IsGenericType && t.BaseType.GetGenericTypeDefinition() == ConfigType)
             .Select(x => Activator.CreateInstance(x) as ITextFileRecordTypeConfiguration);

        foreach(var config in configs) {
            Configs.TryAdd(config!.GetType().BaseType!.GenericTypeArguments[0], config);
        }
    }
}
