using System.Collections.Generic;
using cpGames.core.RapidIoC;

namespace cpGames.core.EntityComponentFramework
{
    public interface IDictionaryProperty<TKey, TValue> : 
        IProperty<Dictionary<TKey, TValue>>,
        IEnumerable<KeyValuePair<TKey, TValue>>
    {
        #region Properties
        long Count { get; }
        bool Empty { get; }
        TValue? this[TKey key] { get; set; }
        TTypedValue? Get<TTypedValue>(TKey key);
        ISignalOutcome<TKey> ElementGetSignal { get; }
        ISignalOutcome<TKey, TValue> BeginElementSetSignal { get; }
        ISignalOutcome<TKey, TValue> EndElementSetSignal { get; }
        ISignalOutcome<TKey, TValue> ElementAddedSignal { get; }
        ISignalOutcome<TKey, TValue> ElementRemovedSignal { get; }
        ISignalOutcome ElementCountChangedSignal { get; }
        #endregion

        #region Methods
        Outcome RemoveElement(TKey key);
        Outcome HasElement(TKey key);
        Outcome GetElement(TKey key, out TValue? value);
        Outcome SetElement(TKey key, TValue value);
        #endregion
    }
}