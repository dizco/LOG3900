using System;
using System.ComponentModel;
using System.Reflection;

namespace PolyPaint.Converters
{
    internal static class EnumConverter
    {
        public static string GetDescription(this Enum enumValue)
        {
            Type enumType = enumValue.GetType();

            string valueName = Enum.GetName(enumType, enumValue);

            if (valueName != null)
            {
                FieldInfo field = enumType.GetField(valueName);
                if (field != null)
                {
                    if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute
                            attribute)
                    {
                        return attribute.Description;
                    }
                }
            }

            return null;
        }
    }
}
