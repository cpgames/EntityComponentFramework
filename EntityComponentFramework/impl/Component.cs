using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using cpGames.core.RapidIoC;
using Newtonsoft.Json;

namespace cpGames.core.EntityComponentFramework.impl
{
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class Component : View, IComponent
    {
        #region Fields
        private readonly List<IProperty> _properties = new();
        #endregion

        #region Properties
        public bool IsConnected { get; private set; }
        #endregion

        #region IComponent Members
        public Entity Entity { get; private set; } = null!;
        public Id Id => Entity.Id;
        public IEnumerable<IProperty> Properties => _properties;
        public ISignalOutcome ConnectedSignal { get; } = new LazySignalOutcome();
        public ISignalOutcome DisconnectedSignal { get; } = new LazySignalOutcome();

        public Outcome Connect(Entity entity)
        {
            if (IsConnected)
            {
                return Outcome.Fail($"Component <{GetType().Name}> is already connected.");
            }
            Entity = entity;
            IsConnected = true;
            return
                RegisterWithContext() &&
                ConnectRequiredComponents() &&
                ConnectRequiredProperties() &&
                ConnectInternal() &&
                ConnectedSignal.DispatchResult();
        }

        public Outcome Disconnect()
        {
            if (!IsConnected)
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
            Entity = null!;
            IsConnected = false;
            _properties.Clear();
            return
                UnregisterFromContextInternal() &&
                DisconnectedSignal.DispatchResult();
        }

        public Outcome GetEntity(out Entity entity)
        {
            if (!IsConnected)
            {
                entity = null!;
                return Outcome.Fail($"Component <{GetType().Name}> is not connected.");
            }
            entity = Entity;
            return Outcome.Success();
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

        private Outcome ConnectRequiredProperties()
        {
            var propertyInfos = GetType().GetProperties().Where(x => x.HasAttribute<PropertyAttribute>());
            foreach (var propertyInfo in propertyInfos)
            {
                var propertyAttribute = propertyInfo.GetAttribute<PropertyAttribute>()!;
                IProperty? property;
                if (propertyAttribute.Type != null)
                {
                    var addPropertyOutcome = Entity.AddProperty(
                        propertyAttribute.Name,
                        propertyAttribute.Type,
                        propertyAttribute.DefaultValue,
                        out property);
                    if (!addPropertyOutcome)
                    {
                        return addPropertyOutcome;
                    }
                }
                else
                {
                    var getPropertyOutcome = Entity.GetProperty(propertyAttribute.Name, out property);
                    if (!getPropertyOutcome)
                    {
                        return getPropertyOutcome;
                    }
                }
                if (!property!.GetType().IsTypeOrDerived(propertyInfo.PropertyType))
                {
                    return Outcome.Fail($"Property <{propertyAttribute.Name}> is not of type <{propertyInfo.PropertyType.Name}> in <{GetType().Name}>.");
                }
                propertyInfo.SetValue(this, property, null);
                _properties.Add(property);
            }
            return Outcome.Success();
        }

        private Outcome ConnectRequiredComponents()
        {
            var propertyInfos = GetType().GetProperties().Where(x => x.HasAttribute<RequiredComponentAttribute>());
            foreach (var propertyInfo in propertyInfos)
            {
                if (propertyInfo.PropertyType == GetType())
                {
                    return Outcome.Fail($"Component <{GetType().Name}> has a property that references itself.");
                }
                var connectPropertyOutcome = ConnectRequiredComponent(Entity, propertyInfo);
                if (!connectPropertyOutcome)
                {
                    return connectPropertyOutcome;
                }
            }
            return Outcome.Success();
        }

        private Outcome ConnectRequiredComponent(Entity target, PropertyInfo propertyInfo)
        {
            var requiredComponentAttribute = propertyInfo.GetAttribute<RequiredComponentAttribute>();
            if (requiredComponentAttribute == null)
            {
                return Outcome.Fail($"{propertyInfo.Name} is missing RequiredComponentAttribute.");
            }
            if (target.GetComponent(propertyInfo.PropertyType, out var component))
            {
                propertyInfo.SetValue(this, component, null);
                return Outcome.Success();
            }
            if (requiredComponentAttribute.Type != default)
            {
                var addComponentOutcome = target.AddComponent(requiredComponentAttribute.Type, out component);
                if (!addComponentOutcome)
                {
                    return addComponentOutcome;
                }
                propertyInfo.SetValue(this, component, null);
                return Outcome.Success();
            }
            return Outcome.Fail($"Component <{GetType().Name}> is missing required component <{propertyInfo.PropertyType.Name}>.");
        }

        private Outcome CheckIfStillRequired(Entity target)
        {
            foreach (var component in target.Components)
            {
                if (component == this)
                {
                    continue;
                }
                if (component.GetType().GetProperties().Any(x => x.PropertyType == GetType() && x.GetValue(component, null) == this))
                {
                    return Outcome.Fail($"Component <{GetType().Name}> can not be removed, it is still required by <{component.GetType().Name}>.");
                }
            }
            return Outcome.Success();
        }
        #endregion
    }
}