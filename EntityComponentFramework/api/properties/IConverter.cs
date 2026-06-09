using System;

namespace cpGames.core.EntityComponentFramework
{
    public interface IConverter
    {
        #region Methods
        bool CanConvert(Type sourceType, Type targetType);
        Outcome Convert(object? data, Type targetType, out object? value);
        #endregion
    }
}
