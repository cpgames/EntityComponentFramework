using System;

namespace cpGames.core.EntityComponentFramework.impl
{
    public class StringToEnumConverter : IConverter
    {
        #region IConverter Members
        public bool CanConvert(Type sourceType, Type targetType)
        {
            return sourceType == typeof(string) && targetType.IsEnum;
        }

        public Outcome Convert(object? data, Type targetType, out object? value)
        {
            value = default;

            if (data is string str)
            {
                try
                {
                    value = Enum.Parse(targetType, str);
                    return Outcome.Success();
                }
                catch (Exception)
                {
                    return Outcome.Fail($"Failed to convert <{str}> to <{targetType.Name}>.");
                }
            }

            return Outcome.Fail($"Cannot convert {data?.GetType().Name} to {targetType.Name}");
        }
        #endregion
    }
}