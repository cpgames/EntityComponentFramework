using System;
using System.Globalization;

namespace cpGames.core.EntityComponentFramework.impl
{
    public class StringToDateTimeConverter : IConverter
    {
        #region IConverter Members
        public bool CanConvert(Type sourceType, Type targetType)
        {
            return sourceType == typeof(string) && targetType == typeof(DateTime);
        }

        public Outcome Convert(object? data, Type targetType, out object? value)
        {
            value = default;

            if (data is string str)
            {
                if (!DateTime.TryParse(
                    str,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var dateTime))
                {
                    return Outcome.Fail($"Failed to convert <{str}> to DateTime.");
                }
                value = dateTime;
                return Outcome.Success();
            }

            return Outcome.Fail($"Cannot convert {data?.GetType().Name} to DateTime");
        }
        #endregion
    }
}