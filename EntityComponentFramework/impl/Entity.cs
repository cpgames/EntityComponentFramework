using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using cpGames.core.RapidIoC;

namespace cpGames.core.EntityComponentFramework.impl
{
    public sealed class Entity : IEnumerable<Entity>, IIdProvider
    {
        #region Fields
        private const int ID_SIZE = 4;
        private static readonly object _syncRoot = new();
        private readonly Id _id;
        private string _name;
        private Entity? _parent;
        private Entity _root;
        private Address _address;
        private readonly List<IComponent> _components = new();
        private readonly Dictionary<Id, Entity> _children = new();
        #endregion

        #region Properties
        public Id Id => _id;
        public string Name
        {
            get => _name;
            set
            {
                if (_name == value)
                {
                    return;
                }
                _name = value;
                NameChangedSignal.Dispatch();
            }
        }
        public Entity? Parent => _parent;
        public Entity Root
        {
            get => _root;
            private set
            {
                if (ReferenceEquals(_root, value))
                {
                    return;
                }
                _root = value;
                foreach (var child in this)
                {
                    child.Root = _root;
                }
            }
        }
        public Address Address => _address;
        public int ChildCount => _children.Count;
        public bool HasChildren => ChildCount > 0;
        public IdGenerator? IdGenerator { get; set; }
        public Entity this[Id id] => _children[id];
        public IEnumerable<Entity> Children => _children.Values;
        public IEnumerable<IComponent> Components => _components;
        public ISignalOutcome<Entity> ChildAddedSignal { get; } = new LazySignalOutcome<Entity>();
        public ISignalOutcome<Id> ChildRemovedSignal { get; } = new LazySignalOutcome<Id>();
        public ISignalOutcome ChildCountChangedSignal { get; } = new LazySignalOutcome();
        public ISignalOutcome ParentChangedSignal { get; } = new LazySignalOutcome();
        public ISignal NameChangedSignal { get; } = new LazySignal();
        public static object SyncRoot => _syncRoot;
        #endregion

        #region Constructors
        public Entity(Id id, string name)
        {
            _id = id;
            _name = name;
            _address = new Address(_id);
            _root = this;
        }
        #endregion

        #region IEnumerable<Entity> Members
        public IEnumerator<Entity> GetEnumerator()
        {
            return _children.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        #region IIdProvider Members
        public bool HasId(Id id)
        {
            return
                HasChildWithId(id);
        }

        public byte IdSize => ID_SIZE;
        #endregion

        #region Methods
        public Outcome GetId(out Id id)
        {
            id = Id;
            return Outcome.Success();
        }

        public Outcome AddComponent(IComponent component)
        {
            if (_components.Any(x => x.GetType() == component.GetType()))
            {
                return Outcome.Fail($"Component <{component.GetType().Name}> already exists in <{this}>.");
            }
            var connectOutcome = component.Connect(this);
            if (!connectOutcome)
            {
                return connectOutcome;
            }
            _components.Add(component);
            return Outcome.Success();
        }

        public Outcome AddComponent(Type componentType)
        {
            if (!typeof(IComponent).IsAssignableFrom(componentType))
            {
                return Outcome.Fail($"Component type <{componentType.Name}> must implement IComponent.");
            }
            if (componentType.IsAbstract || componentType.IsInterface)
            {
                return Outcome.Fail($"Component type <{componentType.Name}> must be non abstract and non interface.");
            }
            if (_components.Any(x => x.GetType() == componentType))
            {
                return Outcome.Fail($"Component of type <{componentType.Name}> already exists in <{this}>.");
            }
            var component = (IComponent)Activator.CreateInstance(componentType);
            var connectOutcome = component.Connect(this);
            if (!connectOutcome)
            {
                return connectOutcome;
            }
            _components.Add(component);
            return Outcome.Success();
        }

        public Outcome AddComponent<TComponent>()
            where TComponent : class, IComponent
        {
            return AddComponent(typeof(TComponent));
        }

        public Outcome AddComponent(Type componentType, out IComponent? component)
        {
            component = null;
            if (!typeof(IComponent).IsAssignableFrom(componentType))
            {
                return Outcome.Fail($"Component type <{componentType.Name}> must implement IComponent.");
            }
            if (componentType.IsAbstract || componentType.IsInterface)
            {
                return Outcome.Fail($"Component type <{componentType.Name}> must be non abstract and non interface.");
            }
            if (_components.Any(x => x.GetType() == componentType))
            {
                return Outcome.Fail($"Component of type <{componentType.Name}> already exists in <{this}>.");
            }
            component = (IComponent)Activator.CreateInstance(componentType);
            var connectOutcome = component.Connect(this);
            if (!connectOutcome)
            {
                return connectOutcome;
            }
            _components.Add(component);
            return Outcome.Success();
        }

        public Outcome AddComponent<TComponent>(out TComponent? component)
            where TComponent : class, IComponent
        {
            var addComponentOutcome = AddComponent(typeof(TComponent), out var componentBase);
            if (!addComponentOutcome)
            {
                component = null;
                return addComponentOutcome;
            }
            component = (TComponent)componentBase!;
            return Outcome.Success();
        }

        public Outcome AddChildWithComponent<TComponent>(Id childId, string childName, out TComponent? component)
            where TComponent : class, IComponent
        {
            component = null;
            return
                AddChild(childId, childName, out var child) &&
                child!.AddComponent(out component);
        }

        public Outcome AddChildWithComponent<TComponent>(string childName, out TComponent? component)
            where TComponent : class, IComponent
        {
            component = null;
            return
                AddChild(childName, out var child) &&
                child!.AddComponent(out component);
        }

        public Outcome RemoveComponent<TComponent>() where TComponent : IComponent
        {
            var component = _components.OfType<TComponent>().FirstOrDefault();
            if (component == null)
            {
                return Outcome.Fail($"Component in entity <{this}> of type <{typeof(TComponent).Name}> already exists.");
            }
            var disconnectResult = component.Disconnect();
            if (!disconnectResult)
            {
                return disconnectResult;
            }
            _components.Remove(component);
            return Outcome.Success();
        }

        public Outcome HasComponent<TComponent>() where TComponent : IComponent
        {
            return !_components.OfType<TComponent>().Any() ?
                Outcome.Fail($"No components in entity <{this}> of type <{typeof(TComponent).Name}>.") :
                Outcome.Success();
        }

        public Outcome GetComponent(Type componentType, out IComponent? component)
        {
            component = _components.FirstOrDefault(x => x.GetType().IsTypeOrDerived(componentType));
            return component == null ?
                Outcome.Fail($"No components in entity <{this}> of type <{componentType.Name}>.") :
                Outcome.Success();
        }

        public Outcome GetComponent<TComponent>(out TComponent? component) where TComponent : IComponent
        {
            lock (_syncRoot)
            {
                component = _components
                    .OfType<TComponent>()
                    .FirstOrDefault();
                return component == null ?
                    Outcome.Fail($"No component of type <{typeof(TComponent)}> exists.") :
                    Outcome.Success();
            }
        }

        public TComponent? GetComponent<TComponent>() where TComponent : IComponent
        {
            lock (_syncRoot)
            {
                return _components
                    .OfType<TComponent>()
                    .FirstOrDefault();
            }
        }

        public Outcome GetComponentInParent<TComponent>(out TComponent? component) where TComponent : IComponent
        {
            lock (_syncRoot)
            {
                component = default;
                var sharedObject = this;
                while (sharedObject != null)
                {
                    component = _components
                        .OfType<TComponent>()
                        .FirstOrDefault();
                    if (component == null)
                    {
                        sharedObject = _parent;
                    }
                    else
                    {
                        break;
                    }
                }
                if (component == null)
                {
                    return Outcome.Fail($"No component of type <{typeof(TComponent)}> exists.");
                }
                return Outcome.Success();
            }
        }

        public IEnumerable<TComponent> GetComponentsInChildren<TComponent>() where TComponent : IComponent
        {
            lock (_syncRoot)
            {
                return Children
                    .Select(x => x.GetComponent<TComponent>())
                    .Where(x => x != null)!;
            }
        }

        public Outcome AddChild(Id id, string name, out Entity? child)
        {
            lock (_syncRoot)
            {
                if (_children.ContainsKey(id))
                {
                    child = null;
                    return Outcome.Fail($"Child with id <{id}> already exists in <{this}>.");
                }
                child = new Entity(id, name);
                _children[child.Id] = child;
                child._parent = this;
                child._root = _root;
                child._address = _address.Append(_id);
                return
                    ChildAddedSignal.DispatchResult(child) &&
                    ChildCountChangedSignal.DispatchResult();
            }
        }

        public Outcome AddChild(Id id, string name)
        {
            lock (_syncRoot)
            {
                if (_children.ContainsKey(id))
                {
                    return Outcome.Fail($"Child with id <{id}> already exists in <{this}>.");
                }
                var child = new Entity(id, name);
                _children[child.Id] = child;
                child._parent = this;
                child._root = _root;
                child._address = _address.Append(_id);
                return
                    ChildAddedSignal.DispatchResult(child) &&
                    ChildCountChangedSignal.DispatchResult();
            }
        }

        public Outcome AddChild(string name, out Entity? child)
        {
            child = null;
            if (IdGenerator == null)
            {
                return Outcome.Fail($"No Id Generator specified in <{this}>.");
            }
            return
                IdGenerator.GenerateId(this, out var id) &&
                AddChild(id, name, out child);
        }

        public Outcome RemoveChild(Id id)
        {
            lock (_syncRoot)
            {
                if (!_children.TryGetValue(id, out var child))
                {
                    return Outcome.Fail($"Child with id <{id}> does not exist in <{this}>.");
                }
                _children.Remove(id);
                child._parent = null;
                child._address = new Address(child.Id);
                child.Root = child;
                return
                    child.ParentChangedSignal.DispatchResult() &&
                    ChildRemovedSignal.DispatchResult(child.Id) &&
                    ChildCountChangedSignal.DispatchResult();
            }
        }

        public Outcome SetParent(Entity? parent)
        {
            lock (_syncRoot)
            {
                if (ReferenceEquals(_parent, parent))
                {
                    return Outcome.Success();
                }
                if (ReferenceEquals(parent, this))
                {
                    return Outcome.Fail($"Child <{this}> can't set parent to itself.");
                }
                if (_parent != null)
                {
                    _parent._children.Remove(Id);
                    var childRemovedOutcome = _parent.ChildRemovedSignal.DispatchResult(Id);
                    if (!childRemovedOutcome)
                    {
                        return childRemovedOutcome;
                    }
                    _parent = null;
                }
                _parent = parent;
                if (parent != null)
                {
                    if (parent._children.ContainsKey(Id))
                    {
                        return Outcome.Fail($"Parent <{parent}> already has child with id <{Id}>.");
                    }
                    _parent = parent;
                    _parent._children[_id] = this;
                    _address = _parent.Address.Append(Id);
                    Root = _parent.Root;
                    var childAddedOutcome = _parent.ChildAddedSignal.DispatchResult(this);
                    if (!childAddedOutcome)
                    {
                        return childAddedOutcome;
                    }
                }
                else
                {
                    _address = new Address(Id);
                    Root = this;
                }
            }
            return ParentChangedSignal.DispatchResult();
        }

        public Outcome Dispose()
        {
            while (_components.Count > 0)
            {
                var disconnectOutcome = _components.First().Disconnect();
                if (!disconnectOutcome)
                {
                    return disconnectOutcome;
                }
            }
            _components.Clear();
            return SetParent(null);
        }

        public Outcome HasChildWithId(Id id)
        {
            return _children.ContainsKey(id) ?
                Outcome.Success() :
                Outcome.Fail($"Child <{id}> does not exist in <{this}>.");
        }

        public Outcome HasChildWithName(string name)
        {
            return Children.Any(x => x.Name == name) ?
                Outcome.Success() :
                Outcome.Fail($"Child <{name}> does not exist in <{this}>.");
        }

        public Outcome GetChildById(Id id, out Entity? child)
        {
            return _children.TryGetValue(id, out child) ?
                Outcome.Success() :
                Outcome.Fail($"Child <{id}> does not exist in <{this}>.");
        }

        public Outcome GetChildByAddress(Address address, out Entity? child)
        {
            return Root.GetChildByAddressInternal(address, 0, out child) ?
                Outcome.Success() :
                Outcome.Fail($"No child with address <{address}> exists in <{Root}>.");
        }

        private bool GetChildByAddressInternal(Address address, int offset, out Entity? child)
        {
            var id = address.GetId(offset);
            if (!id.IsValid || id != Id)
            {
                child = null;
                return false;
            }
            if (Address.IdCount == address.IdCount)
            {
                child = this;
                return true;
            }
            offset++;
            foreach (var value in _children.Values)
            {
                if (value.GetChildByAddressInternal(address, offset, out child))
                {
                    return true;
                }
            }
            child = null;
            return false;
        }

        public Outcome GetChildByName(string name, out Entity? child)
        {
            child = _children.Values.FirstOrDefault(x => x.Name == name);
            return child != null ?
                Outcome.Success() :
                Outcome.Fail($"Child <{name}> does not exist in <{this}>.");
        }

        public override string ToString()
        {
            return $"[{_name}:{Address}]";
        }
        #endregion
    }
}