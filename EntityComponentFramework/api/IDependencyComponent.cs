namespace cpGames.core.EntityComponentFramework
{
    public interface IDependencyComponent
    {
        #region Properties
        IDependencyContainer DependencyContainer { get; }
        #endregion
    }
}