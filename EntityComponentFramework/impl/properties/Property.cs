using System;
using System.Collections.Generic;
using cpGames.core.RapidIoC;

namespace cpGames.core.EntityComponentFramework.impl
{
    public abstract class Property<TValue> : IProperty<TValue>
    {
        #region Fields
        private IProperty? _linkedProperty;
        protected readonly TValue _defaultValue;
        protected TValue _value;
        #endregion

        #region Properties
        protected virtual EqualityComparer<TValue> ValueComparer { get; } = EqualityComparer<TValue>.Default;
        #endregion

        #region Constructors
        protected Property(Entity owner, string name, TValue defaultValue)
        {
            Owner = owner;
            Name = name;
            _defaultValue = defaultValue;
            _value = Clone(defaultValue);
        }
        #endregion

        #region IProperty<TValue> Members
        public string Name { get; }
        public Entity Owner { get; }
        public ISignalOutcome ValueGetSignal { get; } = new LazySignalOutcome();
        public ISignalOutcome<object?> BeginValueSetSignal { get; } = new LazySignalOutcome<object?>();
        public ISignalOutcome<object?> EndValueSetSignal { get; } = new LazySignalOutcome<object?>();
        public Type ValueType => typeof(TValue);

        public Outcome SetData(object? data)
        {
            return
                ConvertToValue(data, out var value) &&
                Set(value!);
        }

        public Outcome GetData(out object? data)
        {
            var valueGetOutcome = ValueGetSignal.DispatchResult();
            if (!valueGetOutcome)
            {
                data = null;
                return valueGetOutcome;
            }
            return
                ConvertToData(_value, out data) &&
                Outcome.Success();
        }

        public Outcome GetValueObj(out object? valueObj)
        {
            var getOutcome = Get(out var value);
            if (!getOutcome)
            {
                valueObj = null;
                return getOutcome;
            }
            valueObj = value;
            return Outcome.Success();
        }

        public TValue Value
        {
            get
            {
                var getOutcome = Get(out var value);
                if (!getOutcome)
                {
                    throw new Exception(getOutcome.ErrorMessage);
                }
                return value!;
            }
            set
            {
                var setOutcome = Set(value);
                if (!setOutcome)
                {
                    throw new Exception(setOutcome.ErrorMessage);
                }
            }
        }

        public Outcome Set(TValue value)
        {
            if (ValueComparer.Equals(_value, value))
            {
                return Outcome.Success();
            }
            return
                BeginValueSetSignal.DispatchResult(value) &&
                Unlink(false) &&
                UpdateValue(value) &&
                EndValueSetSignal.DispatchResult(value);
        }

        public Outcome Set(IProperty<TValue> otherProperty)
        {
            return
                otherProperty.Get(out var otherValue) &&
                Set(otherValue!);
        }

        public Outcome Get(out TValue? value)
        {
            if (!ValueGetSignal.IsDispatching)
            {
                var beginGetOutcome = ValueGetSignal.DispatchResult();
                if (!beginGetOutcome)
                {
                    value = default;
                    return beginGetOutcome;
                }
            }
            value = _value;
            return Outcome.Success();
        }

        public Outcome GetNonDefault(out TValue? value)
        {
            var getOutcome = Get(out value);
            if (!getOutcome)
            {
                return getOutcome;
            }
            if (ValueComparer.Equals(value!, _defaultValue))
            {
                return Outcome.Fail($"<{Owner}:{Name}> Value <{value}> is default value.", this);
            }
            return Outcome.Success();
        }

        public Outcome ValueEquals(object? data)
        {
            var beginGetOutcome = ValueGetSignal.DispatchResult();
            if (!beginGetOutcome)
            {
                return beginGetOutcome;
            }
            if (data == null)
            {
                return _value == null ?
                    Outcome.Success() :
                    Outcome.Fail($"<{Owner}:{Name}> Value <{_value}> does not equal <null>.", this);
            }
            var convertOutcome = ConvertToValue(data, out var value);
            if (!convertOutcome)
            {
                return convertOutcome;
            }
            if (value == null)
            {
                return _value == null ?
                    Outcome.Success() :
                    Outcome.Fail($"<{Owner}:{Name}> Value <{_value}> does not equal <null>.", this);
            }
            return value.Equals(_value) ?
                Outcome.Success() :
                Outcome.Fail($"<{Owner}:{Name}> Value <{_value}> does not equal <{data}>.", this);
        }

        public Outcome ValueEquals(IProperty otherProperty)
        {
            return
                otherProperty.GetData(out var data) &&
                ValueEquals(data);
        }

        public Outcome ValueNotEquals(object? data)
        {
            var beginGetOutcome = ValueGetSignal.DispatchResult();
            if (!beginGetOutcome)
            {
                return beginGetOutcome;
            }
            if (data == null)
            {
                return _value != null ?
                    Outcome.Success() :
                    Outcome.Fail($"<{Owner}:{Name}> Value <{_value}> equals <null>.", this);
            }
            var convertOutcome = ConvertToValue(data, out var value);
            if (!convertOutcome)
            {
                return convertOutcome;
            }
            if (value == null)
            {
                return _value != null ?
                    Outcome.Success() :
                    Outcome.Fail($"<{Owner}:{Name}> Value <{_value}> equals <null>.", this);
            }
            return !value.Equals(_value) ?
                Outcome.Success() :
                Outcome.Fail($"<{Owner}:{Name}> Value <{_value}> equals <{data}>.", this);
        }

        public Outcome ValueNotEquals(IProperty otherProperty)
        {
            return
                otherProperty.GetData(out var data) &&
                ValueNotEquals(data);
        }

        public Outcome IsDefault()
        {
            return ValueEquals(_defaultValue);
        }

        public virtual string ValueToString()
        {
            return _value == null ?
                string.Empty :
                _value!.ToString();
        }

        public Outcome Link(IProperty otherProperty)
        {
            if (otherProperty == _linkedProperty)
            {
                return Outcome.Success();
            }
            return
                Unlink(false) &&
                CanLink(otherProperty) &&
                LinkInternal(otherProperty);
        }

        public Outcome Unlink(bool reset = true)
        {
            if (!IsLinked())
            {
                return Outcome.Success();
            }
            var unlinkOutcome =
                UnlinkInternal(_linkedProperty!) &&
                _linkedProperty!.EndValueSetSignal.RemoveCommand(this);
            if (!unlinkOutcome)
            {
                return unlinkOutcome;
            }
            _linkedProperty = null;
            return reset ?
                Set(_defaultValue) :
                Outcome.Success();
        }

        public Outcome IsLinked()
        {
            return _linkedProperty != null ?
                Outcome.Success() :
                Outcome.Fail($"<{Owner}:{Name}> is not linked.", this);
        }

        public Outcome Equals(TValue value)
        {
            var getValueOutcome = Get(out var myValue);
            if (!getValueOutcome)
            {
                return getValueOutcome;
            }
            if (ValueComparer.Equals(myValue!, value))
            {
                return Outcome.Success();
            }
            return Outcome.Fail($"<{Owner}:{Name}> Value <{myValue}> does not equal <{value}>.", this);
        }

        public Outcome UpdateValue()
        {
            return
                BeginValueSetSignal.DispatchResult(_value) &&
                UpdateValue(_value) &&
                EndValueSetSignal.DispatchResult(_value);
        }
        #endregion

        #region Methods
        private Outcome OnLinkedEndModify(object? data)
        {
            var convertOutcome = ConvertToValue(data, out var value);
            if (!convertOutcome)
            {
                return convertOutcome;
            }
            if (ValueComparer.Equals(_value, value!))
            {
                return Outcome.Success();
            }
            return
                BeginValueSetSignal.DispatchResult(value) &&
                UpdateValue(value!) &&
                EndValueSetSignal.DispatchResult(value);
        }

        protected virtual Outcome CanLink(IProperty otherProperty)
        {
            if (otherProperty.ValueType != ValueType)
            {
                return Outcome.Fail($"Unsupported link between properties of different types: <{ValueType}> and <{otherProperty.ValueType}>.", this);
            }
            return Outcome.Success();
        }

        protected virtual Outcome LinkInternal(IProperty otherProperty)
        {
            _linkedProperty = otherProperty;
            return
                _linkedProperty.GetData(out var linkedValue) &&
                OnLinkedEndModify(linkedValue) &&
                otherProperty.EndValueSetSignal.AddCommand(OnLinkedEndModify, this);
        }

        protected virtual Outcome UnlinkInternal(IProperty otherProperty)
        {
            return Outcome.Success();
        }

        protected virtual Outcome UpdateValue(TValue value)
        {
            _value = value;
            return Outcome.Success();
        }

        protected virtual Outcome ConvertToValue(object? data, out TValue? value)
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
            return Outcome.Fail($"<{Owner}:{Name}> Failed to convert <{data?.GetType().Name}> to <{typeof(TValue).Name}>.", this);
        }

        protected virtual Outcome ConvertToData(TValue? value, out object? data)
        {
            data = value;
            return Outcome.Success();
        }

        protected virtual TValue Clone(TValue value)
        {
            return value;
        }

        public override string ToString()
        {
            return $"{GetType().Name}: {ValueToString()}";
        }
        #endregion
    }
}