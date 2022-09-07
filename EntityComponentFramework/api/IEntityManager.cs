using cpGames.core.EntityComponentFramework.impl;

namespace cpGames.core.EntityComponentFramework
{
    public interface IEntityManager : IComponent
    {
        #region Properties
        Entity WorldEntity { get; }
        IWorld World { get; }
        #endregion

        #region Methods
        Address CreateAddress(Id id);
        #endregion
    }
}