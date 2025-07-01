using System;
using System.Globalization;

namespace cpGames.core.EntityComponentFramework.impl
{
    public class StringToDateTimeConverter : IPropertyConverter<DateTime>
    {
        #region IPropertyConverter<DateTime> Members
        public bool CanConvert(Type type)
        {
            return type == typeof(string);
        }

        public Outcome Convert(object? data, out DateTime value)
        {
            value = default;

            if (data is string str)
            {
                if (!DateTime.TryParse(
                    str,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out value))
                {
                    return Outcome.Fail($"Failed to convert <{str}> to DateTime.");
                }
                return Outcome.Success();
            }

            return Outcome.Fail($"Cannot convert {data?.GetType().Name} to DateTime");
        }
        #endregion
    }
} 