using System;
using System.Collections;
using System.Collections.Generic;
using cpGames.core.RapidIoC;

namespace cpGames.core.EntityComponentFramework.impl
{
    public class ListProperty<TValue> : Property<List<TValue>>, IListProperty<TValue>
    {
        #region Constructors
        public ListProperty(Entity owner, string name, List<TValue> defaultValue) : base(owner, name, defaultValue) {
            EntryAddedSignal.AddCommand(val => 
                EntryObjAddedSignal.DispatchResult(val!) &&
                EntryCountChangedSignal.DispatchResult());
            EntryRemovedSignal.AddCommand(val =>
                EntryObjRemovedSignal.DispatchResult(val!) &&
                EntryCountChangedSignal.DispatchResult());
        }
        public ListProperty(Entity owner, string name) : this(owner, name, new List<TValue>()) { }
        #endregion

        #region IListProperty<TValue> Members
        public long Count => Value.Count;
        public bool Empty => Count == 0;
        public Type EntryType => typeof(TValue);
        public ISignalOutcome<object> EntryObjAddedSignal { get; } = new LazySignalOutcome<object>();
        public ISignalOutcome<object> EntryObjRemovedSignal { get; } = new LazySignalOutcome<object>();
        public ISignalOutcome<TValue> EntryAddedSignal { get; } = new LazySignalOutcome<TValue>();
        public ISignalOutcome<TValue> EntryRemovedSignal { get; } = new LazySignalOutcome<TValue>();
        public TValue this[int index] => Value[index];
        public ISignalOutcome EntryCountChangedSignal { get; } = new LazySignalOutcome();

        public Outcome AddEntryObj(object entryObj)
        {
            return entryObj is TValue entry ?
                AddEntry(entry) :
                Outcome.Fail($"Entry object {entryObj} is not of type {typeof(TValue)}");
        }

        public Outcome RemoveEntryObj(object entryObj)
        {
            return entryObj is TValue entry ?
                RemoveEntry(entry) :
                Outcome.Fail($"Entry object {entryObj} is not of type {typeof(TValue)}");
        }

        public Outcome HasEntryObj(object entryObj)
        {
            return entryObj is TValue entry ?
                HasEntry(entry) :
                Outcome.Fail($"Entry object {entryObj} is not of type {typeof(TValue)}");
        }

        public Outcome AddEntry(TValue entry)
        {
            var getOutcome = Get(out var value);
            if (!getOutcome)
            {
                return getOutcome;
            }
            if (value!.Contains(entry))
            {
                return Outcome.Fail($"List already contains entry {entry}");
            }
            value.Add(entry);
            return EntryAddedSignal.DispatchResult(entry);
        }

        public Outcome RemoveEntry(TValue entry)
        {
            var getOutcome = Get(out var value);
            if (!getOutcome)
            {
                return getOutcome;
            }
            if (!value!.Contains(entry))
            {
                return Outcome.Fail($"List does not contain entry {entry}");
            }
            value.Remove(entry);
            return EntryRemovedSignal.DispatchResult(entry);
        }

        public Outcome HasEntry(TValue entry)
        {
            var getOutcome = Get(out var value);
            if (!getOutcome)
            {
                return getOutcome;
            }
            return value!.Contains(entry) ?
                Outcome.Success() :
                Outcome.Fail($"List does not contain entry {entry}");
        }

        public Outcome Clear()
        {
            var getOutcome = Get(out var value);
            if (!getOutcome)
            {
                return getOutcome;
            }
            value!.Clear();
            return Outcome.Success();
        }

        protected virtual Outcome ConvertEntry(object? data, out TValue? value)
        {
            if (data is TValue entry)
            {
                value = entry;
                return Outcome.Success();
            }
            value = default;
            return Outcome.Fail($"Cannot convert entry {data} to {typeof(TValue)}");
        }

        protected override Outcome ConvertToValue(object? data, out List<TValue>? value)
        {
            if (data is IList list)
            {
                value = new List<TValue>();
                foreach (var entry in list)
                {
                    var convertOutcome = ConvertEntry(entry, out var entryValue);
                    if (!convertOutcome)
                    {
                        value = default;
                        return convertOutcome;
                    }
                    value.Add(entryValue!);
                }
                return Outcome.Success();
            }
            return base.ConvertToValue(data, out value);
        }

        protected override Outcome CanLink(IProperty otherProperty)
        {
            if (otherProperty is IListProperty otherListProperty)
            {
                if (!EntryType.IsAssignableFrom(otherListProperty.EntryType))
                {
                    return Outcome.Fail($"Cannot link list property {Name} to {otherProperty.Name} because entry types are not covariant");
                }
                return Outcome.Success();
            }
            return base.CanLink(otherProperty);
        }

        protected override Outcome LinkInternal(IProperty otherProperty)
        {
            var listProperty = (IListProperty)otherProperty;
            return 
                listProperty.EntryObjAddedSignal.AddCommand(val => EntryAddedSignal.DispatchResult((TValue)val), this) &&
                listProperty.EntryObjRemovedSignal.AddCommand(val => EntryRemovedSignal.DispatchResult((TValue)val), this) &&
                base.LinkInternal(otherProperty);
        }

        protected override Outcome UnlinkInternal(IProperty otherProperty)
        {
            var listProperty = (IListProperty)otherProperty;
            return
                listProperty.EntryObjAddedSignal.RemoveCommand(this) &&
                listProperty.EntryObjRemovedSignal.RemoveCommand(this) &&
                base.UnlinkInternal(otherProperty);
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            return Value.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
    }
}