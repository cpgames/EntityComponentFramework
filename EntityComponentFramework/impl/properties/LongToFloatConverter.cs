using System;

namespace cpGames.core.EntityComponentFramework.impl
{
    public class LongToFloatConverter : IConverter
    {
        #region IConverter Members
        public bool CanConvert(Type sourceType, Type targetType)
        {
            return sourceType == typeof(long) && targetType == typeof(float);
        }

        public Outcome Convert(object? data, Type targetType, out object? value)
        {
            value = default;

            if (data is long l)
            {
                value = (float)l;
                return Outcome.Success();
            }

            return Outcome.Fail($"Cannot convert {data?.GetType().Name} to float");
        }
        #endregion
    }
}