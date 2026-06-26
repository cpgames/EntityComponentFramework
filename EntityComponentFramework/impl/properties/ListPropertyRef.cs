using System;
using System.Collections;
using System.Collections.Generic;
using cpGames.core.RapidIoC;

namespace cpGames.core.EntityComponentFramework.impl
{
    public interface IListPropertyRef<TElementValue> :
        IReadonlyListProperty<TElementValue>,
        IPropRef<IReadonlyListProperty<TElementValue>> { }

    // Read-through reference to an existing list property. Reads and enumeration delegate to the
    // target; the entry signals are forwarded from the target while pointed.
    public class ListPropertyRef<TElementValue> :
        PropertyRef<List<TElementValue>>,
        IListPropertyRef<TElementValue>
    {
        #region Constructors
        public ListPropertyRef(string name) : base(name) { }
        #endregion

        #region Properties
        private IReadonlyListProperty<TElementValue>? List => _target as IReadonlyListProperty<TElementValue>;
        #endregion

        #region IListPropertyRef<TElementValue> Members
        public Outcome Bind(IReadonlyListProperty<TElementValue> target)
        {
            return base.Bind(target);
        }
        #endregion

        #region IReadonlyListProperty<TElementValue> Members
        public long Count => List?.Count ?? 0;
        public bool Empty => List?.Empty ?? true;
        public Type ElementType => typeof(TElementValue);
        public ISignalOutcome<object, int> EntryObjAddedSignal { get; } = new LazySignalOutcome<object, int>();
        public ISignalOutcome<object> EntryObjRemovedSignal { get; } = new LazySignalOutcome<object>();
        public ISignalOutcome EntryCountChangedSignal { get; } = new LazySignalOutcome();
        public ISignalOutcome<TElementValue, int> EntryAddedSignal { get; } = new LazySignalOutcome<TElementValue, int>();
        public ISignalOutcome<TElementValue> EntryRemovedSignal { get; } = new LazySignalOutcome<TElementValue>();

        public TElementValue this[int index]
        {
            get
            {
                if (List == null)
                {
                    throw new InvalidOperationException($"ListPropertyRef <{Name}> is not pointed.");
                }
                return List[index];
            }
        }

        public Outcome HasEntryObj(object entryObj, out bool result)
        {
            if (List == null)
            {
                result = false;
                return Outcome.Success();
            }
            return List.HasEntryObj(entryObj, out result);
        }

        public Outcome GetIndexOf(object entryObj, out int index)
        {
            if (List == null)
            {
                index = -1;
                return Outcome.Success();
            }
            return List.GetIndexOf(entryObj, out index);
        }

        public Outcome HasEntry(TElementValue entry, out bool result)
        {
            if (List == null)
            {
                result = false;
                return Outcome.Success();
            }
            return List.HasEntry(entry, out result);
        }

        public Outcome HasEntry(IReadonlyListProperty<TElementValue>.FilterDelegate filter, out bool result)
        {
            if (List == null)
            {
                result = false;
                return Outcome.Success();
            }
            return List.HasEntry(filter, out result);
        }

        public Outcome FindEntry(IReadonlyListProperty<TElementValue>.FilterDelegate? filter, out TElementValue? entry)
        {
            if (List == null)
            {
                entry = default;
                return Outcome.Fail($"ListPropertyRef <{Name}> is not pointed.");
            }
            return List.FindEntry(filter, out entry);
        }

        public Outcome TryFindEntry(IReadonlyListProperty<TElementValue>.FilterDelegate? filter, out TElementValue? entry)
        {
            if (List == null)
            {
                entry = default;
                return Outcome.Success();
            }
            return List.TryFindEntry(filter, out entry);
        }

        public Outcome FindEntries(IReadonlyListProperty<TElementValue>.FilterDelegate filter, out List<TElementValue> entries)
        {
            if (List == null)
            {
                entries = new List<TElementValue>();
                return Outcome.Success();
            }
            return List.FindEntries(filter, out entries);
        }

        public IEnumerator<TElementValue> GetEnumerator()
        {
            return ((IEnumerable<TElementValue>?)List ?? Array.Empty<TElementValue>()).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        #region Methods
        protected override Outcome AttachToTarget()
        {
            var outcome = base.AttachToTarget();
            if (!outcome || List == null)
            {
                return outcome;
            }
            return
                List.EntryObjAddedSignal.AddCommand(OnTargetEntryObjAdded, this) &&
                List.EntryObjRemovedSignal.AddCommand(OnTargetEntryObjRemoved, this) &&
                List.EntryCountChangedSignal.AddCommand(OnTargetEntryCountChanged, this) &&
                List.EntryAddedSignal.AddCommand(OnTargetEntryAdded, this) &&
                List.EntryRemovedSignal.AddCommand(OnTargetEntryRemoved, this);
        }

        protected override Outcome DetachFromTarget()
        {
            if (List != null)
            {
                var outcome =
                    List.EntryObjAddedSignal.RemoveCommand(this) &&
                    List.EntryObjRemovedSignal.RemoveCommand(this) &&
                    List.EntryCountChangedSignal.RemoveCommand(this) &&
                    List.EntryAddedSignal.RemoveCommand(this) &&
                    List.EntryRemovedSignal.RemoveCommand(this);
                if (!outcome)
                {
                    return outcome;
                }
            }
            return base.DetachFromTarget();
        }

        private Outcome OnTargetEntryObjAdded(object entryObj, int index)
        {
            return EntryObjAddedSignal.DispatchResult(entryObj, index);
        }

        private Outcome OnTargetEntryObjRemoved(object entryObj)
        {
            return EntryObjRemovedSignal.DispatchResult(entryObj);
        }

        private Outcome OnTargetEntryCountChanged()
        {
            return EntryCountChangedSignal.DispatchResult();
        }

        private Outcome OnTargetEntryAdded(TElementValue entry, int index)
        {
            return EntryAddedSignal.DispatchResult(entry, index);
        }

        private Outcome OnTargetEntryRemoved(TElementValue entry)
        {
            return EntryRemovedSignal.DispatchResult(entry);
        }
        #endregion
    }
}
