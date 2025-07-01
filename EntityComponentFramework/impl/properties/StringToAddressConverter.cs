using System;

namespace cpGames.core.EntityComponentFramework.impl
{
    public class StringToAddressConverter : IPropertyConverter<Address>
    {
        public bool CanConvert(Type type)
        {
            return type == typeof(string);
        }

        public Outcome Convert(object? data, out Address value)
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