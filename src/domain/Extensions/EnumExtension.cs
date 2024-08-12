using System.ComponentModel;

namespace Domain.Extensions;

public static class EnumExtension
{
    public static string GetDescription(this Enum value)
    {
        var attr = value.GetAttributes<DescriptionAttribute>();
        return attr is not null && !string.IsNullOrWhiteSpace(attr.Description) ? attr.Description : value.ToString();
    }

    private static TAttribute GetAttributes<TAttribute>(this Enum value)
            where TAttribute : class
        => value.GetType().GetMember(value.ToString()).First().GetCustomAttributes(typeof(TAttribute), false).OfType<TAttribute>().FirstOrDefault()!;

    public static T? StringToEnum<T>(this string str, int defaultValue = 0) where T : Enum
    {
        if (string.IsNullOrWhiteSpace(str))
        {
            return (T)Enum.GetValues(typeof(T)).GetValue(defaultValue)!;
        }

        var type = typeof(T);
        var fields = type.GetFields();
        str = str.Trim();

        foreach (var field in fields)
        {
            var descriptionAttribute = field.GetCustomAttributes(typeof(DescriptionAttribute), false)
                                             .FirstOrDefault() as DescriptionAttribute;

            if (descriptionAttribute is not null && descriptionAttribute.Description.Equals(str, StringComparison.OrdinalIgnoreCase))
            {
                return (T?)field.GetValue(null);
            }
        }

        try
        {
            return (T)Enum.Parse(type, str, true);
        }
        catch
        {
            return (T?)Enum.GetValues(typeof(T)).GetValue(defaultValue);
        }
    }
}