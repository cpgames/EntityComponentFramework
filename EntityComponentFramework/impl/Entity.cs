using System;
using System.Collections.Generic;
using System.Linq;

namespace cpGames.core.EntityComponentFramework.impl
{
    public sealed class Entity
    {
        #region Fields
        private readonly Dictionary<string, IProperty> _properties = new();
        private readonly List<IComponent> _components = new();
        #endregion

        #region Properties
        public Id Id { get; }

        public IProperty this[string name]
        {
            get
            {
                if (!_properties.TryGetValue(name, out var property))
                {
                    throw new Exception($"Property '{name}' not found in {this}.");
                }
                return property;
            }
        }

        public IEnumerable<IProperty> Properties
        {
            get
            {
                lock (SyncRoot)
                {
                    return _properties.Values;
                }
            }
        }

        public IEnumerable<IComponent> Components
        {
            get
            {
                lock (SyncRoot)
                {
                    return _components;
                }
            }
        }

        public object SyncRoot { get; } = new();
        #endregion

        #region Constructors
        public Entity(Id id)
        {
            Id = id;
        }
        #endregion

        #region Methods
        public Outcome AddProperty(
            string name,
            Type type,
            out IProperty? property)
        {
            lock (SyncRoot)
            {
                if (GetProperty(name, out var propertyBase))
                {
                    if (propertyBase!.GetType() == type)
                    {
                        property = propertyBase;
                        return Outcome.Success();
                    }
                    property = default;
                    return Outcome.Fail($"Property <{name}> already exists in <{this}> but is not of type <{type.Name}>.", this);
                }
                if (!typeof(IProperty).IsAssignableFrom(type))
                {
                    property = null;
                    return Outcome.Fail($"Type <{type.Name}> must implement <{nameof(IProperty)}>.", this);
                }
                if (type.IsAbstract)
                {
                    property = null;
                    return Outcome.Fail($"Type <{type.Name}> must not be abstract.", this);
                }
                IProperty? propertyT;
                propertyT = (IProperty)Activator.CreateInstance(type, this, name);
                if (propertyT == null)
                {
                    property = null;
                    return Outcome.Fail($"Property type <{type.Name}> must implement IProperty.", this);
                }
                _properties.Add(name, propertyT);
                property = propertyT;
                return Outcome.Success();
            }
        }

        public Outcome AddProperty<TProperty>(
            string name,
            out TProperty? property)
            where TProperty : class, IProperty
        {
            lock (SyncRoot)
            {
                var outcome = AddProperty(
                    name,
                    typeof(TProperty),
                    out var propertyBase);
                if (!outcome)
                {
                    property = null;
                    return outcome;
                }
                if (propertyBase is not TProperty propertyT)
                {
                    property = null;
                    return Outcome.Fail($"Property <{name}> is not of type <{typeof(TProperty).Name}>.", this);
                }
                property = propertyT;
                return outcome;
            }
        }

        public Outcome HasProperty(string name)
        {
            lock (SyncRoot)
            {
                return _properties.ContainsKey(name) ?
                    Outcome.Success() :
                    Outcome.Fail($"Property <{name}> does not exist in <{this}>.", this);
            }
        }

        public Outcome HasProperty<TProperty>(string name) where TProperty : IProperty
        {
            lock (SyncRoot)
            {
                return _properties.TryGetValue(name, out var property) && property is TProperty ?
                    Outcome.Success() :
                    Outcome.Fail($"Property <{name}> does not exist in <{this}>.", this);
            }
        }

        public Outcome GetProperty(string name, out IProperty? property)
        {
            lock (SyncRoot)
            {
                return _properties.TryGetValue(name, out property) ?
                    Outcome.Success() :
                    Outcome.Fail($"Property <{name}> does not exist in <{this}>.", this);
            }
        }

        public Outcome GetProperty(string name, Type type, out IProperty? property)
        {
            lock (SyncRoot)
            {
                if (!typeof(IProperty).IsAssignableFrom(type))
                {
                    property = null;
                    return Outcome.Fail($"Type <{type.Name}> must implement <{nameof(IProperty)}>.", this);
                }
                var getPropertyOutcome = GetProperty(name, out var propertyBase);
                if (!getPropertyOutcome)
                {
                    property = null;
                    return getPropertyOutcome;
                }
                if (!propertyBase!.GetType().IsTypeOrDerived(type))
                {
                    property = null;
                    return Outcome.Fail($"Property <{name}> is not of type <{type.Name}> in <{this}>.", this);
                }
                property = propertyBase;
                return Outcome.Success();
            }
        }

        public Outcome GetProperty<TProperty>(string name, out TProperty? property) where TProperty : IProperty
        {
            var getPropertyOutcome = GetProperty(name, out var propertyBase);
            if (!getPropertyOutcome)
            {
                property = default;
                return getPropertyOutcome;
            }
            if (propertyBase is not TProperty propertyT)
            {
                property = default;
                return Outcome.Fail($"Property <{name}> is not of type <{typeof(TProperty).Name}> in <{this}>.", this);
            }
            property = propertyT;
            return Outcome.Success();
        }

        public Outcome SetPropertyValue<TProperty>(string name, object? value)
            where TProperty : class, IProperty
        {
            lock (SyncRoot)
            {
                if (GetProperty<TProperty>(name, out var existingProperty))
                {
                    var setDataOutcome = existingProperty!.SetData(value);
                    return setDataOutcome;
                }
                return
                    AddProperty<TProperty>(name, out var newProperty) &&
                    newProperty!.SetData(value);
            }
        }

        public Outcome GetPropertyValue<TProperty, TValue>(string name, out TValue? value)
            where TProperty : class, IProperty<TValue>
        {
            lock (SyncRoot)
            {
                var getPropertyOutcome = GetProperty<TProperty>(name, out var property);
                if (!getPropertyOutcome)
                {
                    value = default;
                    return getPropertyOutcome;
                }
                return property!.Get(out value);
            }
        }

        public Outcome AddComponent(IComponent component)
        {
            lock (SyncRoot)
            {
                if (_components.Any(x => x.GetType() == component.GetType()))
                {
                    return Outcome.Fail($"Component <{component.GetType().Name}> already exists in <{this}>.", this);
                }
                _components.Add(component);
                var connectOutcome = component.Connect(this);
                if (!connectOutcome)
                {
                    _components.Remove(component);
                    return connectOutcome;
                }
                return Outcome.Success();
            }
        }

        public Outcome AddComponent(Type componentType, params object[] args)
        {
            lock (SyncRoot)
            {
                if (!typeof(IComponent).IsAssignableFrom(componentType))
                {
                    return Outcome.Fail($"Component type <{componentType.Name}> must implement IComponent.", this);
                }
                if (componentType.IsAbstract || componentType.IsInterface)
                {
                    return Outcome.Fail($"Component type <{componentType.Name}> must be non abstract and non interface.", this);
                }
                if (_components.Any(x => x.GetType() == componentType))
                {
                    return Outcome.Fail($"Component of type <{componentType.Name}> already exists in <{this}>.", this);
                }
                var component = (IComponent)Activator.CreateInstance(componentType, args);
                _components.Add(component);
                var connectOutcome = component.Connect(this);
                if (!connectOutcome)
                {
                    _components.Remove(component);
                    return connectOutcome;
                }
                return Outcome.Success();
            }
        }

        public Outcome AddComponent<TComponent>(params object[] args)
            where TComponent : class, IComponent
        {
            return AddComponent(typeof(TComponent), args);
        }

        public Outcome AddComponent(Type componentType, out IComponent? component, params object[] args)
        {
            lock (SyncRoot)
            {
                component = null;
                if (!typeof(IComponent).IsAssignableFrom(componentType))
                {
                    return Outcome.Fail($"Component type <{componentType.Name}> must implement IComponent.", this);
                }
                if (componentType.IsAbstract || componentType.IsInterface)
                {
                    return Outcome.Fail($"Component type <{componentType.Name}> must be non abstract and non interface.", this);
                }
                if (_components.Any(x => x.GetType() == componentType))
                {
                    return Outcome.Fail($"Component of type <{componentType.Name}> already exists in <{this}>.", this);
                }
                component = (IComponent)Activator.CreateInstance(componentType, args);
                _components.Add(component);
                var connectOutcome = component.Connect(this);
                if (!connectOutcome)
                {
                    _components.Remove(component);
                    return connectOutcome;
                }
                return Outcome.Success();
            }
        }

        public Outcome AddComponent<TComponent>(out TComponent? component, params object[] args)
            where TComponent : class, IComponent
        {
            var addComponentOutcome = AddComponent(typeof(TComponent), out var componentBase, args);
            if (!addComponentOutcome)
            {
                component = null;
                return addComponentOutcome;
            }
            component = (TComponent)componentBase!;
            return Outcome.Success();
        }

        public Outcome AddComponent<TComponent, TComponentImpl>(out TComponent? component, params object[] args)
            where TComponent : class, IComponent
            where TComponentImpl : TComponent
        {
            var addComponentOutcome = AddComponent(typeof(TComponentImpl), out var componentBase, args);
            if (!addComponentOutcome)
            {
                component = null;
                return addComponentOutcome;
            }
            component = (TComponent)componentBase!;
            return Outcome.Success();
        }

        public Outcome AddComponent<TComponent>(Type componentType, out TComponent? component, params object[] args)
            where TComponent : class, IComponent
        {
            var addComponentOutcome = AddComponent(componentType, out var componentBase, args);
            if (!addComponentOutcome)
            {
                component = null;
                return addComponentOutcome;
            }
            component = (TComponent)componentBase!;
            return Outcome.Success();
        }

        public Outcome RemoveComponent(IComponent component)
        {
            lock (SyncRoot)
            {
                if (!_components.Contains(component))
                {
                    return Outcome.Fail($"Component <{component.GetType().Name}> does not exist in <{this}>.", this);
                }
                var disconnectResult = component.Disconnect();
                if (!disconnectResult)
                {
                    return disconnectResult;
                }
                _components.Remove(component);
                return Outcome.Success();
            }
        }

        public Outcome RemoveComponent<TComponent>() where TComponent : IComponent
        {
            lock (SyncRoot)
            {
                var component = _components.OfType<TComponent>().FirstOrDefault();
                if (component == null)
                {
                    return Outcome.Fail($"Component in entity <{this}> of type <{typeof(TComponent).Name}> already exists.", this);
                }
                return RemoveComponent(component);
            }
        }

        public Outcome HasComponent<TComponent>() where TComponent : IComponent
        {
            lock (SyncRoot)
            {
                return !_components.OfType<TComponent>().Any() ?
                    Outcome.Fail($"No components in entity <{this}> of type <{typeof(TComponent).Name}>.", this) :
                    Outcome.Success();
            }
        }

        public Outcome GetComponent(Type componentType, out IComponent? component)
        {
            lock (SyncRoot)
            {
                component = _components.FirstOrDefault(x => x.GetType().IsTypeOrDerived(componentType));
            }
            // ReSharper disable once ConvertIfStatementToReturnStatement for debugging
            if (component == null)
            {
                return Outcome.Fail($"No components in entity <{this}> of type <{componentType.Name}>.", this);
            }
            return Outcome.Success();
        }

        public Outcome GetComponent<TComponent>(out TComponent? component) where TComponent : IComponent
        {
            lock (SyncRoot)
            {
                component = _components
                    .OfType<TComponent>()
                    .FirstOrDefault();
                // ReSharper disable once ConvertIfStatementToReturnStatement for debugging
                if (component == null)
                {
                    return Outcome.Fail($"No component of type <{typeof(TComponent)}> exists.", this);
                }
                return Outcome.Success();
            }
        }

        public TComponent? GetComponent<TComponent>() where TComponent : IComponent
        {
            lock (SyncRoot)
            {
                return _components
                    .OfType<TComponent>()
                    .FirstOrDefault();
            }
        }

        public Outcome Dispose()
        {
            lock (SyncRoot)
            {
                while (_components.Count > 0)
                {
                    var disconnectOutcome = _components[_components.Count - 1].Disconnect();
                    if (!disconnectOutcome)
                    {
                        return disconnectOutcome;
                    }
                    _components.Remove(_components[_components.Count - 1]);
                }
                _components.Clear();
                foreach (var property in _properties.Values)
                {
                    var unlinkOutcome = property.Unlink();
                    if (!unlinkOutcome)
                    {
                        return unlinkOutcome;
                    }
                }
            }
            return Outcome.Success();
        }

        public Outcome HasPropertyWithPropertyValues(Dictionary<string, object?> propertyValues)
        {
            foreach (var propertyValue in propertyValues)
            {
                var getPropertyOutcome = GetProperty(propertyValue.Key, out var property);
                if (!getPropertyOutcome)
                {
                    return getPropertyOutcome;
                }
                var equalsOutcome = property!.ValueEquals(propertyValue.Value);
                if (!equalsOutcome)
                {
                    return equalsOutcome;
                }
            }
            return Outcome.Success();
        }
        #endregion
    }
}