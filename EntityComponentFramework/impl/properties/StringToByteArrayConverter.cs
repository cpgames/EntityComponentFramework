using System;
using System.Text;

namespace cpGames.core.EntityComponentFramework.impl
{
    public class StringToByteArrayConverter : IPropertyConverter<byte[]>
    {
        #region IPropertyConverter<byte[]> Members
        public bool CanConvert(Type type)
        {
            return type == typeof(string);
        }

        public Outcome Convert(object? data, out byte[]? value)
        {
            value = null;

            if (data is string str)
            {
                value = Encoding.Default.GetBytes(str);
                return Outcome.Success();
            }

            return Outcome.Fail($"Cannot convert {data?.GetType().Name} to byte array");
        }
        #endregion
    }
} 