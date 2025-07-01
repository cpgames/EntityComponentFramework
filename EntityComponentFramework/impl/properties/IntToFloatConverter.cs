using System;

namespace cpGames.core.EntityComponentFramework.impl
{
    public class IntToFloatConverter : IPropertyConverter<float>
    {
        #region IPropertyConverter<float> Members
        public bool CanConvert(Type type)
        {
            return type == typeof(int);
        }

        public Outcome Convert(object? data, out float value)
        {
            value = default;

            if (data is int i)
            {
                value = (float)i;
                return Outcome.Success();
            }

            return Outcome.Fail($"Cannot convert {data?.GetType().Name} to float");
        }
        #endregion
    }
} 