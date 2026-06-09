using System;

namespace cpGames.core.EntityComponentFramework.impl
{
    public class StringToIdConverter : IConverter
    {
        #region IConverter Members
        public bool CanConvert(Type sourceType, Type targetType)
        {
            return sourceType == typeof(string) && targetType == typeof(Id);
        }

        public Outcome Convert(object? data, Type targetType, out object? value)
        {
            value = Id.INVALID;

            if (data is string str)
            {
                var outcome = Id.TryParse(str, out var id);
                value = id;
                return outcome;
            }

            return Outcome.Fail($"Cannot convert {data?.GetType().Name} to Id");
        }
        #endregion
    }
}