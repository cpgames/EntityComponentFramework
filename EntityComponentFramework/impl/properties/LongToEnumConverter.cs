using System;

namespace cpGames.core.EntityComponentFramework.impl
{
    public class LongToEnumConverter : IConverter
    {
        #region IConverter Members
        public bool CanConvert(Type sourceType, Type targetType)
        {
            return sourceType == typeof(long) && targetType.IsEnum;
        }

        public Outcome Convert(object? data, Type targetType, out object? value)
        {
            value = default;

            if (data is long dataLong)
            {
                try
                {
                    value = Enum.ToObject(targetType, dataLong);
                    return Outcome.Success();
                }
                catch (Exception e)
                {
                    return Outcome.Fail($"Failed to convert <{dataLong}> to <{targetType.Name}>: {e.Message}");
                }
            }

            return Outcome.Fail($"Cannot convert {data?.GetType().Name} to {targetType.Name}");
        }
        #endregion
    }
}