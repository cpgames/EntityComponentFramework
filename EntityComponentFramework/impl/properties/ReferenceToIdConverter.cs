using System;

namespace cpGames.core.EntityComponentFramework.impl
{
    public class ReferenceToIdConverter : IPropertyConverter<Id>
    {
        #region IPropertyConverter<Id> Members
        public bool CanConvert(Type type)
        {
            return typeof(IComponent).IsAssignableFrom(type);
        }

        public Outcome Convert(object? data, out Id value)
        {
            value = data is IComponent component ? component.Id : Id.INVALID;
            return Outcome.Success();
        }
        #endregion
    }
}
