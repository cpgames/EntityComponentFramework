using System.Collections;
using System.Collections.Generic;
using cpGames.core.RapidIoC;

namespace cpGames.core.EntityComponentFramework.impl
{
    public class ListProperty<TValue> : Property<List<TValue>>, IListProperty<TValue>
    {
        #region Constructors
        public ListProperty(Entity owner, string name, List<TValue> defaultValue) : base(owner, name, defaultValue) { }
        public ListProperty(Entity owner, string name) : base(owner, name, new List<TValue>()) { }
        #endregion

        #region IListProperty<TValue> Members
        public long Count => Value.Count;
        public bool Empty => Count == 0;
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
            return
                EntryAddedSignal.DispatchResult(entry) &&
                EntryCountChangedSignal.DispatchResult();
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
            return
                EntryRemovedSignal.DispatchResult(entry) &&
                EntryCountChangedSignal.DispatchResult();
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