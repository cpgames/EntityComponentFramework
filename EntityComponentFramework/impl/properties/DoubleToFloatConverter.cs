using System;

namespace cpGames.core.EntityComponentFramework.impl
{
    public class DoubleToFloatConverter : IPropertyConverter<float>
    {
        #region IPropertyConverter<float> Members
        public bool CanConvert(Type type)
        {
            return type == typeof(double);
        }

        public Outcome Convert(object? data, out float value)
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