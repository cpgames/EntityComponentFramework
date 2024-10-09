namespace cpGames.core.EntityComponentFramework
{
    public interface ILongProperty : IProperty<long>
    {
        #region Methods
        Outcome Add(long value);
        Outcome Subtract(long value);
        #endregion
    }
}