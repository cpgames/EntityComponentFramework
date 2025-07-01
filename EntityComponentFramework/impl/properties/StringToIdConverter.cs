using System;

namespace cpGames.core.EntityComponentFramework.impl
{
    public class StringToIdConverter : IPropertyConverter<Id>
    {
        #region IPropertyConverter<Id> Members
        public bool CanConvert(Type type)
        {
            return type == typeof(string);
        }

        public Outcome Convert(object? data, out Id value)
        {
            value = Id.INVALID;

            if (data is string str)
            {
                return Id.TryParse(str, out value);
            }

            return Outcome.Fail($"Cannot convert {data?.GetType().Name} to Id");
        }
        #endregion
    }
} 