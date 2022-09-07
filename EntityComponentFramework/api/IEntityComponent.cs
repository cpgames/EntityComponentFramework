using System.Collections.Generic;
using cpGames.core.EntityComponentFramework.impl;

namespace cpGames.core.EntityComponentFramework
{
    public interface IEntityComponent : IComponent
    {
        #region Properties
        Entity? WorldEntity { get; }
        IWorld? World { get; }
        IStringProperty Name { get; }
        Id EntityType { get; }
        byte EntityTypeByte { get; }
        IEnumerable<Entity> Properties { get; }
        IEnumerable<Entity> Dependencies { get; }
        #endregion
    }
}