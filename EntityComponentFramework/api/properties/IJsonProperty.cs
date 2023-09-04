namespace cpGames.core.EntityComponentFramework
{
    public interface IJsonProperty<TModel> : IProperty<TModel?>
        where TModel : class
    {
        #region Methods
        Outcome Clone(out TModel? value);
        Outcome RefreshJson();
        #endregion
    }
}