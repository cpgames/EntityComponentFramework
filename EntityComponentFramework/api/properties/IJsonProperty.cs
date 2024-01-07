﻿namespace cpGames.core.EntityComponentFramework
{
    public interface IJsonProperty : IProperty
    {
        #region Methods
        Outcome RefreshJson();
        #endregion
    }

    public interface IJsonProperty<TModel> : IProperty<TModel?>, IJsonProperty
        where TModel : class
    {
        #region Methods
        Outcome Clone(out TModel? value);
        #endregion
    }
}