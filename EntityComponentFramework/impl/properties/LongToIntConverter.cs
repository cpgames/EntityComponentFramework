using System;

namespace cpGames.core.EntityComponentFramework.impl
{
    public class LongToIntConverter : IConverter
    {
        #region IConverter Members
        public bool CanConvert(Type sourceType, Type targetType)
        {
            return sourceType == typeof(long) && targetType == typeof(int);
        }

        public Outcome Convert(object? data, Type targetType, out object? value)
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