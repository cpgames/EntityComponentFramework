using System;
using System.Collections;
using System.Collections.Generic;
using cpGames.core.RapidIoC;

namespace cpGames.core.EntityComponentFramework
{
    public interface IReadonlyListProperty : IReadonlyProperty, IEnumerable
    {
        #region Properties
        long Count { get; }
        bool Empty { get; }
        Type ElementType { get; }
        ISignalOutcome<object, int> EntryObjAddedSignal { get; }
        ISignalOutcome<object> EntryObjRemovedSignal { get; }
        ISignalOutcome EntryCountChangedSignal { get; }
        #endregion

        #region Methods
        Outcome HasEntryObj(object entryObj, out bool result);
        Outcome GetIndexOf(object entryObj, out int index);
        #endregion
    }

    public interface IListProperty : IReadonlyListProperty, IProperty
    {
        #region Properties
        bool AllowDuplicates { get; set; }
        #endregion

        #region Methods
        Outcome AddEntryObj(object entryObj);
        Outcome RemoveEntryObj(object entryObj);
        Outcome RemoveEntryAtIndex(int index);
        Outcome InsertEntryObj(object entryObj, int index);
        #endregion
    }

    public interface IReadonlyListProperty<TElementValue> : IReadonlyProperty<List<TElementValue>>, IReadonlyListProperty, IEnumerable<TElementValue>
    {
        #region Nested type: FilterDelegate
        public delegate Outcome FilterDelegate(TElementValue element, out bool result);
        #endregion

        #region Properties
        ISignalOutcome<TElementValue, int> EntryAddedSignal { get; }
        ISignalOutcome<TElementValue> EntryRemovedSignal { get; }
        TElementValue this[int index] { get; }
        #endregion

        #region Methods
        Outcome HasEntry(TElementValue entry, out bool result);
        Outcome HasEntry(FilterDelegate filter, out bool result);
        Outcome FindEntry(FilterDelegate? filter, out TElementValue? entry);
        Outcome TryFindEntry(FilterDelegate? filter, out TElementValue? entry);
        Outcome FindEntries(FilterDelegate filter, out List<TElementValue> entries);
        #endregion
    }

    public interface IListProperty<TElementValue> : IReadonlyListProperty<TElementValue>, IListProperty, IProperty<List<TElementValue>>
    {
        #region Methods
        Outcome AddEntry(TElementValue entry);
        Outcome RemoveEntry(TElementValue entry);
        Outcome TryRemoveEntry(TElementValue entry);
        Outcome RemoveEntry(FilterDelegate filter);
        Outcome Clear();
        Outcome InsertEntry(TElementValue entry, int index);
        #endregion
    }
}