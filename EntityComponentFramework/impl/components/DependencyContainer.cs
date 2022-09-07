using System.Collections;
using System.Collections.Generic;
using System.Linq;
using cpGames.core.RapidIoC;
using ECS;

namespace cpGames.core.EntityComponentFramework.impl
{
    public class  DependencyContainer : Component, IDependencyContainer
    {
        #region Fields
        protected readonly Dictionary<Address, Entity> _dependencies = new Dictionary<Address, Entity>();
        #endregion

        #region IDependencyContainer Members
        public string Name => Entity.Name;
        public int Count => _dependencies.Count;
        public bool Empty => _dependencies.Count == 0;
        public IEnumerable<Entity> Dependencies => _dependencies.Values;
        public Entity this[Address address] => _dependencies[address];
        public ISignalOutcome<Entity> DependencyAddedSignal { get; } = new LazySignalOutcome<Entity>();
        public ISignalOutcome<Address> DependencyRemovedSignal { get; } = new LazySignalOutcome<Address>();
        public ISignalOutcome DependencyCountChangedSignal { get; } = new LazySignalOutcome();

        public Outcome Add(Entity entity)
        {
            if (_dependencies.ContainsKey(entity.Address))
            {
                return Outcome.Fail($"Dependency with address <{entity.Address}> already exists in dependency container <{this}>.");
            }
            _dependencies[entity.Address] = entity;
            return
                DependencyAddedSignal.DispatchResult(entity) &&
                DependencyCountChangedSignal.DispatchResult();
        }

        public Outcome Remove(Address address)
        {
            if (!_dependencies.Remove(address))
            {
                return Outcome.Fail($"Dependency <{address}> does not exist in dependency container <{this}>.");
            }
            return
                DependencyRemovedSignal.DispatchResult(address) &&
                DependencyCountChangedSignal.DispatchResult();
        }

        public IEnumerable<TEntityComponent> GetDependenciesComponents<TEntityComponent>() where TEntityComponent : class, IEntityComponent
        {
            return
                Dependencies
                    .Where(x => x.HasComponent<TEntityComponent>())
                    .Select(x => x.GetComponent<TEntityComponent>());
        }

        public Outcome Contains(Address address)
        {
            return _dependencies.ContainsKey(address) ?
                Outcome.Success() :
                Outcome.Fail("No");
        }

        public Outcome GetDependencyByAddress(Address address, out Entity entity)
        {
            return _dependencies.TryGetValue(address, out entity) ?
                Outcome.Success() :
                Outcome.Fail($"No dependency <{address}> found in dependency container <{this}>.");
        }

        public Outcome GetDependencyComponentByAddress<TEntityComponent>(Address address, out TEntityComponent entityComponent) where TEntityComponent : class, IEntityComponent
        {
            entityComponent = null;
            return
                GetDependencyByAddress(address, out var entity) &&
                entity.GetComponent(out entityComponent);
        }

        public Outcome GetDependencyById<TEntityManager>(Id id, out Entity entity) where TEntityManager : IEntityManager
        {
            entity = null;
            return
                Rapid.GetBindingValue<TEntityManager>(GameIds.WorldKey, out var manager) &&
                GetDependencyByAddress(manager.CreateAddress(id), out entity);
        }

        public Outcome GetDependencyComponentById<TEntityComponent, TEntityManager>(Id id, out TEntityComponent entityComponent) where TEntityComponent : class, IEntityComponent where TEntityManager : IEntityManager
        {
            entityComponent = null;
            return
                Rapid.GetBindingValue<TEntityManager>(GameIds.WorldKey, out var manager) &&
                GetDependencyComponentByAddress(manager.CreateAddress(id), out entityComponent);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<Entity> GetEnumerator()
        {
            return _dependencies.Values.GetEnumerator();
        }

        public Outcome GetDependencyByName(string name, out Entity entity)
        {
            entity = Dependencies.FirstOrDefault(x => x.Name == name);
            return entity != null ?
                Outcome.Success() :
                Outcome.Fail($"Dependency <{name}> does not exist in dependency container <{this}>.");
        }

        public Outcome GetDependencyComponentByName<TEntityComponent>(string name, out TEntityComponent entityComponent) where TEntityComponent : class, IEntityComponent
        {
            entityComponent = null;
            return
                GetDependencyByName(name, out var entity) &&
                entity.GetComponent(out entityComponent);
        }

        public Outcome GetAnyDependencyComponent<TEntityComponent>(out TEntityComponent entityComponent) where TEntityComponent : class, IEntityComponent
        {
            entityComponent = Dependencies
                .FirstOrDefault(x => x.HasComponent<TEntityComponent>())
                .GetComponent<TEntityComponent>();
            return entityComponent != null ?
                Outcome.Success() :
                Outcome.Fail($"Dependency container <{this}> does not have any entity with component <{typeof(TEntityComponent).Name}>.");
        }
        #endregion

        #region Methods
        public override string ToString()
        {
            return $"{Name}:{Entity.Address}";
        }
        #endregion
    }
}