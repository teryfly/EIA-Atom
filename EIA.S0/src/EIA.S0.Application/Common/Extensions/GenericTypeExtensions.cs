namespace EIA.S0.Application.Common.Extensions;

/// <summary>
/// 得到类型名称扩展.
/// </summary>
public static class GenericTypeExtensions
{
    private static string GetGenericTypeName(this Type type)
    {
        string typeName;

        if (type.IsGenericType)
        {
            var genericTypes = string.Join(",", type.GetGenericArguments().Select(t => t.Name).ToArray());
            typeName = $"{type.Name.Remove(type.Name.IndexOf('`'))}<{genericTypes}>";
        }
        else
        {
            typeName = type.Name;
        }

        return typeName;
    }

    /// <summary>
    /// 得到类型名称（包括泛型）.
    /// </summary>
    /// <param name="object"></param>
    /// <returns></returns>
    public static string GetGenericTypeName(this object @object)
    {
        return @object.GetType().GetGenericTypeName();
    }
}