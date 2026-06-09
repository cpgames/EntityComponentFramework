using System;

namespace cpGames.core.EntityComponentFramework.impl
{
    public class DoubleToFloatConverter : IConverter
    {
        #region IConverter Members
        public bool CanConvert(Type sourceType, Type targetType)
        {
            return sourceType == typeof(double) && targetType == typeof(float);
        }

        public Outcome Convert(object? data, Type targetType, out object? value)
        {
            value = default;

            if (data is double d)
            {
                value = (float)d;
                return Outcome.Success();
            }

            return Outcome.Fail($"Cannot convert {data?.GetType().Name} to float");
        }
        #endregion
    }
}