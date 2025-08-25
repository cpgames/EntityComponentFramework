using System;

namespace cpGames.core.EntityComponentFramework.impl
{
    public class ByteToBoolConverter : IPropertyConverter<bool>
    {
        public bool CanConvert(Type type)
        {
            return type == typeof(byte) || type == typeof(sbyte);
        }

        public Outcome Convert(object? data, out bool value)
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
