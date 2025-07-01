using System;

namespace cpGames.core.EntityComponentFramework.impl
{
    public class LongToIntConverter : IPropertyConverter<int>
    {
        #region IPropertyConverter<int> Members
        public bool CanConvert(Type type)
        {
            return type == typeof(long);
        }

        public Outcome Convert(object? data, out int value)
        {
            value = default;

            if (data is long longValue)
            {
                value = (int)longValue;
                return Outcome.Success();
            }

            return Outcome.Fail($"Cannot convert {data?.GetType().Name} to int");
        }
        #endregion
    }
} 