using System;

namespace cpGames.core.EntityComponentFramework.impl
{
    public class LongToBoolConverter : IPropertyConverter<bool>
    {
        public bool CanConvert(Type type)
        {
            return type == typeof(long);
        }

        public Outcome Convert(object? data, out bool value)
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