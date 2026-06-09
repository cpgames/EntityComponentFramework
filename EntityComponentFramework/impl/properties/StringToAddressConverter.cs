using System;

namespace cpGames.core.EntityComponentFramework.impl
{
    public class StringToAddressConverter : IConverter
    {
        public bool CanConvert(Type sourceType, Type targetType)
        {
            return sourceType == typeof(string) && targetType == typeof(Address);
        }

        public Outcome Convert(object? data, Type targetType, out object? value)
        {
            value = Address.INVALID;

            if (data is string str)
            {
                value = new Address(str);
                return Outcome.Success();
            }

            return Outcome.Fail($"Cannot convert {data?.GetType().Name} to Address");
        }
    }
}