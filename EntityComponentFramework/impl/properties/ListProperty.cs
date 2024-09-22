using System;
using System.Collections;
using System.Collections.Generic;
using cpGames.core.RapidIoC;

namespace cpGames.core.EntityComponentFramework.impl
{
    public class ListProperty<TElementValue> : Property<List<TElementValue>>, IListProperty<TElementValue>
    {
        #region Constructors
        public ListProperty(Entity owner, string name) : base(owner, name, new List<TElementValue>())
        {
            EntryAddedSignal.AddCommand(val =>
                EntryObjAddedSignal.DispatchResult(val!) &&
                EntryCountChangedSignal.DispatchResult());
            EntryRemovedSignal.AddCommand(val =>
                EntryObjRemovedSignal.DispatchResult(val!) &&
                EntryCountChangedSignal.DispatchResult());
        }
        #endregion

        #region IListProperty<TElementValue> Members
        public long Count => Value.Count;
        public bool Empty => Count == 0;
        public Type ElementType => typeof(TElementValue);
        public ISignalOutcome<object> EntryObjAddedSignal { get; } = new LazySignalOutcome<object>();
        public ISignalOutcome<object> EntryObjRemovedSignal { get; } = new LazySignalOutcome<object>();
        public ISignalOutcome<TElementValue> EntryAddedSignal { get; } = new LazySignalOutcome<TElementValue>();
        public ISignalOutcome<TElementValue> EntryRemovedSignal { get; } = new LazySignalOutcome<TElementValue>();
        public TElementValue this[int index] => Value[index];
        public ISignalOutcome EntryCountChangedSignal { get; } = new LazySignalOutcome();

        public Outcome AddEntryObj(object entryObj)
        {
            return entryObj is TElementValue entry ?
                AddEntry(entry) :
                Outcome.Fail($"Entry object {entryObj} is not of type {typeof(TElementValue)}", this);
        }

        public Outcome RemoveEntryObj(object entryObj)
        {
            return entryObj is TElementValue entry ?
                RemoveEntry(entry) :
                Outcome.Fail($"Entry object {entryObj} is not of type {typeof(TElementValue)}", this);
        }

        public Outcome HasEntryObj(object entryObj, out bool result)
        {
            result = false;
            return entryObj is TElementValue entry ?
                HasEntry(entry, out result) :
                Outcome.Fail($"Entry object {entryObj} is not of type {typeof(TElementValue)}", this);
        }

        public Outcome AddEntry(TElementValue entry)
        {
            var getOutcome = Get(out var value);
            if (!getOutcome)
            {
                return getOutcome;
            }
            if (value!.Contains(entry))
            {
                return Outcome.Fail($"List already contains entry {entry}", this);
            }
            value.Add(entry);
            return EntryAddedSignal.DispatchResult(entry);
        }

        public Outcome RemoveEntry(TElementValue entry)
        {
            var getOutcome = Get(out var value);
            if (!getOutcome)
            {
                return getOutcome;
            }
            if (!value!.Contains(entry))
            {
                return Outcome.Fail($"List does not contain entry {entry}", this);
            }
            value.Remove(entry);
            return EntryRemovedSignal.DispatchResult(entry);
        }

        public Outcome HasEntry(TElementValue entry, out bool result)
        {
            result = false;
            var outcome = Get(out var value);
            if (!outcome)
            {
                return outcome.Append(this);
            }
            result = value!.Contains(entry);
            return Outcome.Success();
        }

        public Outcome HasEntry(IListProperty<TElementValue>.FilterDelegate filter, out bool result)
        {
            result = false;
            var outcome = Get(out var value);
            if (!outcome)
            {
                return outcome.Append(this);
            }
            foreach (var entry in value!)
            {
                outcome = filter(entry, out result);
                if (!outcome)
                {
                    return outcome.Append(this);
                }
                if (result)
                {
                    return Outcome.Success();
                }
            }
            return Outcome.Success();
        }

        public Outcome FindEntry(IListProperty<TElementValue>.FilterDelegate? filter, out TElementValue? entry)
        {
            entry = default;
            var outcome = Get(out var values);
            if (!outcome)
            {
                return outcome.Append(this);
            }
            if (filter == null)
            {
                if (values!.Count > 0)
                {
                    entry = values[0];
                    return Outcome.Success();
                }
                return Outcome.Fail("No entry found in list", this);
            }
            foreach (var value in values!)
            {
                outcome = filter(value, out var result);
                if (!outcome)
                {
                    entry = default;
                    return outcome.Append(this);
                }
                if (result)
                {
                    entry = value;
                    return Outcome.Success();
                }
            }
            return Outcome.Fail("No entry found in list", this);
        }

        public Outcome FindEntries(IListProperty<TElementValue>.FilterDelegate filter, out List<TElementValue>? entries)
        {
            entries = new List<TElementValue>();
            var outcome = Get(out var values);
            if (!outcome)
            {
                return outcome.Append(this);
            }
            foreach (var value in values!)
            {
                outcome = filter(value, out var result);
                if (!outcome)
                {
                    entries = default;
                    return outcome.Append(this);
                }
                if (result)
                {
                    entries.Add(value);
                }
            }
            return Outcome.Success();
        }

        public Outcome Clear()
        {
            var outcome = Get(out var value);
            if (!outcome)
            {
                return outcome.Append(this);
            }
            while (value!.Count > 0)
            {
                outcome = RemoveEntry(value[0]);
                if (!outcome)
                {
                    return outcome.Append(this);
                }
            }
            return Outcome.Success();
        }

        public IEnumerator<TElementValue> GetEnumerator()
        {
            return Value.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ValueToString()
        {
            return _value.Count.ToString();
        }
        #endregion

        #region Methods
        protected virtual Outcome ConvertEntry(object? data, out TElementValue? value)
        {
            if (data is TElementValue entry)
            {
                value = entry;
                return Outcome.Success();
            }
            value = default;
            return Outcome.Fail($"Cannot convert entry {data} to {typeof(TElementValue)}", this);
        }

        protected override Outcome ConvertToValue(object? data, out List<TElementValue>? value)
        {
            if (data is IList list)
            {
                value = new List<TElementValue>();
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
                if (!ElementType.IsAssignableFrom(otherListProperty.ElementType))
                {
                    return Outcome.Fail($"Cannot link list property {Name} to {otherProperty.Name} because entry types are not covariant", this);
                }
                return Outcome.Success();
            }
            return base.CanLink(otherProperty);
        }

        protected override Outcome LinkInternal(IProperty otherProperty)
        {
            var listProperty = (IListProperty)otherProperty;
            return
                listProperty.EntryObjAddedSignal.AddCommand(AddEntryObj, this) &&
                listProperty.EntryObjRemovedSignal.AddCommand(RemoveEntryObj, this) &&
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

        protected override List<TElementValue> Clone(List<TElementValue> value)
        {
            return new List<TElementValue>(value);
        }
        #endregion
    }
}