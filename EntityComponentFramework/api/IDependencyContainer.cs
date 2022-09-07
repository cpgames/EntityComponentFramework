using System.Collections.Generic;
using cpGames.core.RapidIoC;
using ECS;

namespace cpGames.core.EntityComponentFramework
{
    public interface IDependencyContainer : IEnumerable<Entity>, IComponent
    {
        #region Properties
        string Name { get; }
        int Count { get; }
        bool Empty { get; }
        IEnumerable<Entity> Dependencies { get; }
        Entity this[Address address] { get; }
        ISignalOutcome<Entity> DependencyAddedSignal { get; }
        ISignalOutcome<Address> DependencyRemovedSignal { get; }
        ISignalOutcome DependencyCountChangedSignal { get; }
        #endregion

        #region Methods
        Outcome Add(Entity entity);
        Outcome Remove(Address address);
        IEnumerable<TEntityComponent> GetDependenciesComponents<TEntityComponent>() where TEntityComponent : class, IEntityComponent;
        Outcome Contains(Address address);
        Outcome GetDependencyByAddress(Address address, out Entity entity);
        Outcome GetDependencyComponentByAddress<TEntityComponent>(Address address, out TEntityComponent entityComponent) where TEntityComponent : class, IEntityComponent;
        Outcome GetDependencyById<TEntityManager>(Id id, out Entity entity) where TEntityManager : IEntityManager;

        Outcome GetDependencyComponentById<TEntityComponent, TEntityManager>(Id id, out TEntityComponent entityComponent)
            where TEntityComponent : class, IEntityComponent
            where TEntityManager : IEntityManager;

        Outcome GetDependencyByName(string name, out Entity entity);
        Outcome GetDependencyComponentByName<TEntityComponent>(string name, out TEntityComponent entityComponent) where TEntityComponent : class, IEntityComponent;
        Outcome GetAnyDependencyComponent<TEntityComponent>(out TEntityComponent entityComponent) where TEntityComponent : class, IEntityComponent;
        #endregion
    }
}