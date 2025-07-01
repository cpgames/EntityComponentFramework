using System;

namespace cpGames.core.EntityComponentFramework.impl
{
    public class StringToEnumConverter<TEnum> : IPropertyConverter<TEnum>
        where TEnum : struct
    {
        #region IPropertyConverter<TEnum> Members
        public bool CanConvert(Type type)
        {
            return type == typeof(string);
        }

        public Outcome Convert(object? data, out TEnum value)
        {
            value = default;

            if (data is string str)
            {
                return Enum.TryParse(str, out value) ?
                    Outcome.Success() :
                    Outcome.Fail($"Failed to convert <{str}> to <{typeof(TEnum).Name}>.");
            }

            return Outcome.Fail($"Cannot convert {data?.GetType().Name} to {typeof(TEnum).Name}");
        }
        #endregion
    }
} 