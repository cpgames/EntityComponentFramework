using System;
using System.Collections;
using System.Collections.Generic;
using cpGames.core.RapidIoC;

namespace cpGames.core.EntityComponentFramework
{
    public interface IListProperty : IProperty, IEnumerable
    {
        #region Properties
        long Count { get; }
        bool Empty { get; }
        Type ElementType { get; }
        ISignalOutcome<object> EntryObjAddedSignal { get; }
        ISignalOutcome<object> EntryObjRemovedSignal { get; }
        ISignalOutcome EntryCountChangedSignal { get; }
        #endregion

        #region Methods
        Outcome AddEntryObj(object entryObj);
        Outcome RemoveEntryObj(object entryObj);
        Outcome HasEntryObj(object entryObj);
        #endregion
    }

    public interface IListProperty<TElementValue> : IProperty<List<TElementValue>>, IListProperty, IEnumerable<TElementValue>
    {
        #region Properties
        ISignalOutcome<TElementValue> EntryAddedSignal { get; }
        ISignalOutcome<TElementValue> EntryRemovedSignal { get; }
        TElementValue this[int index] { get; }
        #endregion

        #region Methods
        Outcome AddEntry(TElementValue entry);
        Outcome RemoveEntry(TElementValue entry);
        Outcome HasEntry(TElementValue entry);
        Outcome Clear();
        #endregion
    }
}