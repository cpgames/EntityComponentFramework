using System;

namespace cpGames.core.EntityComponentFramework.impl
{
    public class LongToFloatConverter : IPropertyConverter<float>
    {
        #region IPropertyConverter<float> Members
        public bool CanConvert(Type type)
        {
            return type == typeof(long);
        }

        public Outcome Convert(object? data, out float value)
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