using System;

namespace cpGames.core.EntityComponentFramework.impl
{
    public class IntToBoolConverter : IConverter
    {
        public bool CanConvert(Type sourceType, Type targetType)
        {
            return sourceType == typeof(int) && targetType == typeof(bool);
        }

        public Outcome Convert(object? data, Type targetType, out object? value)
        {
            value = false;
            if (data is int i)
            {
                value = i != 0;
                return Outcome.Success();
            }
            return Outcome.Fail($"Cannot convert {data?.GetType().Name} to bool");
        }
    }
}