using System;

namespace cpGames.core.EntityComponentFramework.impl
{
    public class LongToBoolConverter : IConverter
    {
        public bool CanConvert(Type sourceType, Type targetType)
        {
            return sourceType == typeof(long) && targetType == typeof(bool);
        }

        public Outcome Convert(object? data, Type targetType, out object? value)
        {
            value = false;
            if (data is long l)
            {
                value = l != 0;
                return Outcome.Success();
            }
            return Outcome.Fail($"Cannot convert {data?.GetType().Name} to bool");
        }
    }
}