using System;

namespace cpGames.core.EntityComponentFramework.impl
{
    public class LongToEnumConverter<TEnum> : IPropertyConverter<TEnum>
        where TEnum : struct
    {
        #region IPropertyConverter<TEnum> Members
        public bool CanConvert(Type type)
        {
            return type == typeof(long);
        }

        public Outcome Convert(object? data, out TEnum value)
        {
            value = default;

            if (data is long dataLong)
            {
                try
                {
                    value = (TEnum)Enum.GetValues(typeof(TEnum)).GetValue(dataLong);
                    return Outcome.Success();
                }
                catch (Exception e)
                {
                    return Outcome.Fail($"Failed to convert <{dataLong}> to <{typeof(TEnum).Name}>: {e.Message}");
                }
            }

            return Outcome.Fail($"Cannot convert {data?.GetType().Name} to {typeof(TEnum).Name}");
        }
        #endregion
    }
} 