using System.Linq;
using System.Reflection;
using cpGames.core.RapidIoC;

namespace cpGames.core.EntityComponentFramework.impl
{
    public abstract class Component : View, IComponent
    {
        #region IComponent Members
        public Entity? Entity { get; private set; }
        public ISignalOutcome ConnectedSignal { get; } = new LazySignalOutcome();
        public ISignalOutcome DisconnectedSignal { get; } = new LazySignalOutcome();

        public Outcome Connect(Entity entity)
        {
            if (Entity != null)
            {
                return Outcome.Fail($"Component <{GetType().Name}> is already connected.");
            }
            Entity = entity;
            return
                ConnectRequiredComponents() &&
                ConnectInternal() &&
                ConnectedSignal.DispatchResult();
        }

        public Outcome Disconnect()
        {
            if (Entity == null)
            {
                return Outcome.Fail($"Component <{GetType().Name}> is already disconnected.");
            }
            var disconnectInternalOutcome =
                CheckIfStillRequired(Entity) &&
                DisconnectInternal();
            if (!disconnectInternalOutcome)
            {
                return disconnectInternalOutcome;
            }
            Entity = null;
            return DisconnectedSignal.DispatchResult();
        }
        #endregion

        #region Methods
        protected virtual Outcome ConnectInternal()
        {
            return Outcome.Success();
        }

        protected virtual Outcome DisconnectInternal()
        {
            return Outcome.Success();
        }

        private Outcome ConnectRequiredComponents()
        {
            var properties = GetType().GetProperties().Where(x => x.HasAttribute<RequiredComponentAttribute>());
            foreach (var property in properties)
            {
                if (property.PropertyType == GetType())
                {
                    return Outcome.Fail($"Component <{GetType().Name}> has a property that references itself.");
                }
                var connectPropertyOutcome = ConnectRequiredProperty(Entity, property);
                if (!connectPropertyOutcome)
                {
                    return connectPropertyOutcome;
                }
            }
            return Outcome.Success();
        }

        private Outcome ConnectRequiredProperty(Entity target, PropertyInfo property)
        {
            var requiredComponentAttribute = property.GetAttribute<RequiredComponentAttribute>();
            if (requiredComponentAttribute == null)
            {
                return Outcome.Fail($"{property.Name} is missing RequiredComponentAttribute.");
            }
            if (target.GetComponent(property.PropertyType, out var component))
            {
                property.SetValue(this, component, null);
                return Outcome.Success();
            }
            if (requiredComponentAttribute.addIfMissing)
            {
                var addComponentOutcome = target.AddComponent(property.PropertyType, out component);
                if (!target.AddComponent(property.PropertyType, out component))
                {
                    return addComponentOutcome;
                }
                property.SetValue(this, component, null);
                return Outcome.Success();
            }
            if (requiredComponentAttribute.searchParent && target.Parent != null)
            {
                return ConnectRequiredProperty(target.Parent, property);
            }
            return Outcome.Fail($"Component <{GetType().Name}> is missing required component <{property.PropertyType.Name}>.");
        }

        private Outcome CheckIfStillRequired(Entity target)
        {
            foreach (var component in target.Components)
            {
                if (component == this)
                {
                    continue;
                }

                var properties = component.GetType().GetProperties()
                    .Where(x => x.HasAttribute<RequiredComponentAttribute>());
                if (properties.Any(x => GetType().IsTypeOrDerived(x.PropertyType)))
                {
                    return Outcome.Fail($"Component <{GetType().Name}> can not be removed, it is still required by <{component.GetType().Name}>.");
                }
            }

            foreach (var child in target)
            {
                var checkIfStillRequiredOutcome = CheckIfStillRequired(child);
                if (!checkIfStillRequiredOutcome)
                {
                    return checkIfStillRequiredOutcome;
                }
            }
            return Outcome.Success();
        }
        #endregion
    }
}