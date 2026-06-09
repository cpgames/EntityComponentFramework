using System;
using System.Text;

namespace cpGames.core.EntityComponentFramework.impl
{
    public class StringToByteArrayConverter : IConverter
    {
        #region IConverter Members
        public bool CanConvert(Type sourceType, Type targetType)
        {
            return sourceType == typeof(string) && targetType == typeof(byte[]);
        }

        public Outcome Convert(object? data, Type targetType, out object? value)
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