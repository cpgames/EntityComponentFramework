namespace cpGames.core.EntityComponentFramework
{
    public interface IJsonProperty : IProperty
    {
        #region Methods
        Outcome RefreshJson();
        #endregion
    }

    public interface IJsonProperty<TModel> : IProperty<TModel?>, IJsonProperty
    {
        #region Methods
        Outcome Clone(out TModel? value);

        Outcome GetDerivedNonDefault<TDerivedModel>(out TDerivedModel? derivedModel)
            where TDerivedModel : class, TModel;
        #endregion
    }
}