namespace cpGames.core.EntityComponentFramework
{
    public interface IIntProperty : IProperty<int>
    {
        #region Methods
        Outcome Add(int value);
        Outcome Subtract(int value);
        #endregion
    }
}