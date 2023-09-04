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
    public interface IListProperty<TValue> : IProperty<List<TValue>>, IListProperty, IEnumerable<TValue>
    {
        #region Properties
        ISignalOutcome<TValue> EntryAddedSignal { get; }
        ISignalOutcome<TValue> EntryRemovedSignal { get; }
        TValue this[int index] { get; }
        #endregion

        #region Methods
        Outcome AddEntry(TValue entry);
        Outcome RemoveEntry(TValue entry);
        Outcome HasEntry(TValue entry);
        #endregion
    }
}