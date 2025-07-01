using System;

namespace cpGames.core.EntityComponentFramework
{
    public interface IPropertyConverter<TValue>
    {
        #region Methods
        bool CanConvert(Type type);
        Outcome Convert(object? data, out TValue? value);
        #endregion
    }
}