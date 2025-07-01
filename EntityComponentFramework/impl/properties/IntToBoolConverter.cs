using System;

namespace cpGames.core.EntityComponentFramework.impl
{
    public class IntToBoolConverter : IPropertyConverter<bool>
    {
        public bool CanConvert(Type type)
        {
            return type == typeof(int);
        }

        public Outcome Convert(object? data, out bool value)
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