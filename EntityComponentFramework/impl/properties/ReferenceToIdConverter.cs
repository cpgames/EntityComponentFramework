using System;

namespace cpGames.core.EntityComponentFramework.impl
{
    public class ReferenceToIdConverter : IConverter
    {
        #region IConverter Members
        public bool CanConvert(Type sourceType, Type targetType)
        {
            return sourceType == typeof(Entity) && targetType == typeof(Id);
        }

        public Outcome Convert(object? data, Type targetType, out object? value)
        {
            value = data is Entity entity ? entity.Id : Id.INVALID;
            return Outcome.Success();
        }
        #endregion
    }
}
