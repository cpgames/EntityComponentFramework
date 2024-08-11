namespace cpGames.core.EntityComponentFramework
{
    public interface IFloatProperty : IProperty<float>
    {
        #region Methods
        Outcome Add(float value);
        Outcome Subtract(float value);
        #endregion
    }
}