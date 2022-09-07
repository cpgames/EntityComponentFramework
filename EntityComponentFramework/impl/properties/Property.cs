using System.Collections.Generic;
using cpGames.core.RapidIoC;
using ECS;

namespace cpGames.core.EntityComponentFramework.impl
{
    public abstract class Property<TValue> : Component, IProperty<TValue>
    {
        #region Fields
        private IProperty<TValue> _connectedProperty;
        protected TValue _value;
        #endregion

        #region Properties
        protected virtual EqualityComparer<TValue> ValueComparer { get; } = EqualityComparer<TValue>.Default;
        #endregion

        #region IProperty<TValue> Members
        public string Name => Entity.Name;
        public virtual object Data => _value;
        public Entity Owner => Entity.Parent;
        public ISignalOutcome BeginModifySignal { get; } = new LazySignalOutcome();
        public ISignalOutcome EndModifySignal { get; } = new LazySignalOutcome();

        public Outcome SetObject(object data)
        {
            return
                Convert(data, out var value) &&
                Set(value);
        }

        public TValue Value => _value;

        public Outcome Set(TValue value)
        {
            if (ValueComparer.Equals(_value, value))
            {
                return Outcome.Success();
            }
            return
                BeginModifySignal.DispatchResult() &&
                UpdateValue(value) &&
                EndModifySignal.DispatchResult();
        }

        public TValue Get()
        {
            return _value;
        }

        public bool ValueEquals(object data)
        {
            return ((TValue)data).Equals(_value);
        }

        public virtual string ValueToString()
        {
            return _value == null ? string.Empty : _value.ToString();
        }

        public Outcome ConnectToProperty(IProperty<TValue> otherProperty)
        {
            if (otherProperty == _connectedProperty)
            {
                return Outcome.Success();
            }
            if (_connectedProperty != null)
            {
                var endModifyOutcome = _connectedProperty.EndModifySignal.RemoveCommand(this);
                if (!endModifyOutcome)
                {
                    return endModifyOutcome;
                }
            }
            _connectedProperty = otherProperty;
            return
                _connectedProperty.EndModifySignal.AddCommand(OnConnectedPropertyEndModify, this) &&
                Set(_connectedProperty.Get());
        }

        public Outcome DisconnectProperties()
        {
            if (_connectedProperty == null)
            {
                return Outcome.Success();
            }
            var endModifyOutcome = _connectedProperty.EndModifySignal.RemoveCommand(this);
            if (!endModifyOutcome)
            {
                return endModifyOutcome;
            }
            _connectedProperty = null;
            return Set(default);
        }

        public Outcome UpdateValue()
        {
            return
                BeginModifySignal.DispatchResult() &&
                UpdateValue(_value) &&
                EndModifySignal.DispatchResult();
        }
        #endregion

        #region Methods
        private Outcome OnConnectedPropertyEndModify()
        {
            return Set(_connectedProperty.Get());
        }

        protected virtual Outcome UpdateValue(TValue value)
        {
            _value = value;
            return Outcome.Success();
        }

        protected virtual Outcome Convert(object data, out TValue value)
        {
            if (data == null)
            {
                value = default;
                return Outcome.Success();
            }
            if (data is TValue)
            {
                value = (TValue)data;
                return Outcome.Success();
            }
            value = default;
            return Outcome.Fail($"<{Owner}:{Name}> Failed to convert <{data.GetType().Name}> to <{typeof(TValue).Name}>.");
        }

        public override string ToString()
        {
            return ValueToString();
        }
        #endregion
    }
}