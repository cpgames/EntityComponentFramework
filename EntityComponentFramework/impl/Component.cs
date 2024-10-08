﻿using System.Collections.Generic;
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

        #region IComponent Members
        public bool IsConnected { get; private set; }
        public Entity Entity { get; private set; } = null!;

        public Id Id => Entity.Id;

        public IEnumerable<IProperty> Properties => _properties;
        public ISignalOutcome<IComponent> BeginConnectedSignal { get; } = new LazySignalOutcome<IComponent>();
        public ISignalOutcome<IComponent> EndConnectedSignal { get; } = new LazySignalOutcome<IComponent>();
        public ISignalOutcome<IComponent> BeginDisconnectedSignal { get; } = new LazySignalOutcome<IComponent>();
        public ISignalOutcome<IComponent> EndDisconnectedSignal { get; } = new LazySignalOutcome<IComponent>();

        public Outcome Connect(Entity entity)
        {
            if (IsConnected)
            {
                return Outcome.Fail($"Component <{GetType().Name}> is already connected.", this);
            }
            Entity = entity;
            IsConnected = true;
            return
                BeginConnectedSignal.DispatchResult(this) &&
                RegisterWithContext() &&
                ConnectRequiredProperties() &&
                ConnectRequiredComponents() &&
                ConnectInternal() &&
                EndConnectedSignal.DispatchResult(this);
        }

        public Outcome Disconnect()
        {
            if (!IsConnected)
            {
                return Outcome.Fail($"Component <{GetType().Name}> is already disconnected.", this);
            }
            var disconnectInternalOutcome =
                BeginDisconnectedSignal.DispatchResult(this) &&
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
                UnregisterFromContext() &&
                EndDisconnectedSignal.DispatchResult(this);
        }

        public Outcome GetEntity(out Entity entity)
        {
            if (!IsConnected)
            {
                entity = null!;
                return Outcome.Fail($"Component <{GetType().Name}> is not connected.", this);
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
            var propertyInfos = GetType().GetProperties()
                .Where(x => x.HasAttribute<PropertyAttribute>())
                .OrderBy(x => x.GetAttribute<PropertyAttribute>()!.Order);
            foreach (var propertyInfo in propertyInfos)
            {
                var propertyAttribute = propertyInfo.GetAttribute<PropertyAttribute>()!;
                IProperty? property;
                if (propertyAttribute.Type != null && !Entity.HasProperty(propertyAttribute.Name))
                {
                    var addPropertyOutcome = Entity.AddProperty(
                        propertyAttribute.Name,
                        propertyAttribute.Type,
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
                    return Outcome.Fail($"Property <{propertyAttribute.Name}> is not of type <{propertyInfo.PropertyType.Name}> in <{GetType().Name}>.", this);
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
                    return Outcome.Fail($"Component <{GetType().Name}> has a property that references itself.", this);
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
                return Outcome.Fail($"{propertyInfo.Name} is missing RequiredComponentAttribute.", this);
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
            return Outcome.Fail($"Component <{GetType().Name}> is missing required component <{propertyInfo.PropertyType.Name}>.", this);
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
                    return Outcome.Fail($"Component <{GetType().Name}> can not be removed, it is still required by <{component.GetType().Name}>.", this);
                }
            }
            return Outcome.Success();
        }
        #endregion
    }
}