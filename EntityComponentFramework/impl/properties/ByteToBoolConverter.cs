using System;

namespace cpGames.core.EntityComponentFramework.impl
{
    public class ByteToBoolConverter : IConverter
    {
        public bool CanConvert(Type sourceType, Type targetType)
        {
            return (sourceType == typeof(byte) || sourceType == typeof(sbyte)) && targetType == typeof(bool);
        }

        public Outcome Convert(object? data, Type targetType, out object? value)
        {
            value = false;
            if (data is byte b)
            {
                value = b != 0;
                return Outcome.Success();
            }
            if (data is sbyte sb)
            {
                value = sb != 0;
                return Outcome.Success();
            }
            return Outcome.Fail($"Cannot convert {data?.GetType().Name} to bool");
        }
    }
}
