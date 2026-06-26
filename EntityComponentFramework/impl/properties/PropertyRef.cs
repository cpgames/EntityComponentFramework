using System;
using cpGames.core.RapidIoC;

namespace cpGames.core.EntityComponentFramework.impl
{
    public interface IPropRef<TTarget> where TTarget : IReadonlyProperty
    {
        #region Properties
        bool IsBound { get; }
        #endregion

        #region Methods
        Outcome Bind(TTarget target);
        Outcome Unbind();
        #endregion
    }

    public interface IPropertyRef<TValue> :
        IReadonlyProperty<TValue>,
        IPropRef<IReadonlyProperty<TValue>> { }

    // A pure read-through reference to an existing property. Unlike Link, it stores no value
    // and performs no copy/convert: reads delegate to the target, and the change signals are
    // forwarded from the target while bound (re-binding moves the forwarding subscriptions).
    public class PropertyRef<TValue> : IPropertyRef<TValue>
    {
        #region Fields
        private readonly string _name;
        protected IReadonlyProperty<TValue>? _target;
        #endregion

        #region Constructors
        public PropertyRef(string name)
        {
            _name = name;
        }
        #endregion

        #region IPropertyRef<TValue> Members
        public bool IsBound => _target != null;

        public Outcome Bind(IReadonlyProperty<TValue> target)
        {
            if (ReferenceEquals(target, _target))
            {
                return Outcome.Success();
            }
            return
                DetachFromTarget() &&
                SetTarget(target);
        }

        public Outcome Unbind()
        {
            if (_target == null)
            {
                return Outcome.Success();
            }
            return
                DetachFromTarget() &&
                SetTarget(null);
        }
        #endregion

        #region IReadonlyProperty<TValue> Members
        public string Name => _target?.Name ?? _name;
        public Entity Owner => _target?.Owner!;
        public Type ValueType => typeof(TValue);
        public ISignalOutcome ValueGetSignal { get; } = new LazySignalOutcome { IgnoreRecursiveDispatch = true };
        public ISignalOutcome<object?> BeginValueSetSignal { get; } = new LazySignalOutcome<object?>();
        public ISignalOutcome<object?> EndValueSetSignal { get; } = new LazySignalOutcome<object?>();

        public Outcome Get(out TValue? value)
        {
            if (_target == null)
            {
                value = default;
                return Outcome.Success();
            }
            return _target.Get(out value);
        }

        public Outcome GetNonDefault(out TValue? value)
        {
            if (_target == null)
            {
                value = default;
                return Outcome.Fail($"PropertyRef <{_name}> is not bound.");
            }
            return _target.GetNonDefault(out value);
        }

        public Outcome Equals(TValue value, out bool result)
        {
            if (_target == null)
            {
                result = false;
                return Outcome.Fail($"PropertyRef <{_name}> is not bound.");
            }
            return _target.Equals(value, out result);
        }

        public Outcome GetData(out object? data)
        {
            if (_target == null)
            {
                data = null;
                return Outcome.Success();
            }
            return _target.GetData(out data);
        }

        public Outcome GetValueObj(out object? valueObj)
        {
            if (_target == null)
            {
                valueObj = null;
                return Outcome.Success();
            }
            return _target.GetValueObj(out valueObj);
        }

        public Outcome ValueEquals(object? value, out bool result)
        {
            if (_target == null)
            {
                result = false;
                return Outcome.Success();
            }
            return _target.ValueEquals(value, out result);
        }

        public Outcome ValueEquals(IProperty otherProperty, out bool result)
        {
            if (_target == null)
            {
                result = false;
                return Outcome.Success();
            }
            return _target.ValueEquals(otherProperty, out result);
        }

        public Outcome ValueNotEquals(object? value)
        {
            return _target?.ValueNotEquals(value) ?? Outcome.Success();
        }

        public Outcome ValueNotEquals(IProperty otherProperty)
        {
            return _target?.ValueNotEquals(otherProperty) ?? Outcome.Success();
        }

        public Outcome IsDefault(out bool result)
        {
            if (_target == null)
            {
                result = true;
                return Outcome.Success();
            }
            return _target.IsDefault(out result);
        }

        public string ValueToString()
        {
            return _target?.ValueToString() ?? string.Empty;
        }
        #endregion

        #region Methods
        // After any bind change, broadcast the ref's new current value (default when unbound) so
        // owners watching the ref update their local state - they don't care whether it changed
        // because of a re-bind or an unbind, only that the observed value moved.
        private Outcome SetTarget(IReadonlyProperty<TValue>? target)
        {
            _target = target;
            return
                AttachToTarget() &&
                NotifyValueChanged();
        }

        protected virtual Outcome NotifyValueChanged()
        {
            var outcome = Get(out var value);
            if (!outcome)
            {
                return outcome;
            }
            return
                BeginValueSetSignal.DispatchResult(value) &&
                EndValueSetSignal.DispatchResult(value);
        }

        protected virtual Outcome AttachToTarget()
        {
            if (_target == null)
            {
                return Outcome.Success();
            }
            return
                _target.ValueGetSignal.AddCommand(OnTargetValueGet, this) &&
                _target.BeginValueSetSignal.AddCommand(OnTargetBeginSet, this) &&
                _target.EndValueSetSignal.AddCommand(OnTargetEndSet, this);
        }

        protected virtual Outcome DetachFromTarget()
        {
            if (_target == null)
            {
                return Outcome.Success();
            }
            return
                _target.ValueGetSignal.RemoveCommand(this) &&
                _target.BeginValueSetSignal.RemoveCommand(this) &&
                _target.EndValueSetSignal.RemoveCommand(this);
        }

        private Outcome OnTargetValueGet()
        {
            return ValueGetSignal.DispatchResult();
        }

        private Outcome OnTargetBeginSet(object? value)
        {
            return BeginValueSetSignal.DispatchResult(value);
        }

        private Outcome OnTargetEndSet(object? value)
        {
            return EndValueSetSignal.DispatchResult(value);
        }
        #endregion
    }
}
