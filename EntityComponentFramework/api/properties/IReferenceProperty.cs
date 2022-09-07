using cpGames.core.EntityComponentFramework.impl;

namespace cpGames.core.EntityComponentFramework
{
    public interface IReferenceProperty : IProperty<Address>
    {
        #region Properties
        Entity TargetEntity { get; }
        Address TargetAddress { get; }
        Id TargetId { get; }
        #endregion

        #region Methods
        Outcome SetId<TEntityManager>(Id id) where TEntityManager : IEntityManager;
        Outcome HasTarget();
        Outcome GetTargetEntity(out Entity target);

        Outcome GetTargetComponent<TComponent>(out TComponent targetComponent)
            where TComponent : class, IComponent;
        #endregion
    }
}