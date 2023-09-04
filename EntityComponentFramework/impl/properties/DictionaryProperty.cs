using System;
using System.Collections;
using System.Collections.Generic;
using cpGames.core.RapidIoC;

namespace cpGames.core.EntityComponentFramework.impl
{
    public class DictionaryProperty<TKey, TValue> : Property<Dictionary<TKey, TValue>>, IDictionaryProperty<TKey, TValue>
    {
        #region Constructors
        public DictionaryProperty(Entity owner, string name, Dictionary<TKey, TValue> defaultValue) : base(owner, name, defaultValue) { }
        public DictionaryProperty(Entity owner, string name) : base(owner, name, new Dictionary<TKey, TValue>()) { }
        #endregion

        #region IDictionaryProperty<TKey,TValue> Members
        public long Count => Value.Count;
        public bool Empty => Count == 0;
        public TValue? this[TKey key]
        {
            get
            {
                var elementGetOutcome = ElementGetSignal.DispatchResult(key);
                if (!elementGetOutcome)
                {
                    return default;
                }
                return Value[key];
            }
            set
            {
                if (value == null)
                {
                    throw new Exception("Cannot set a dictionary element to null");
                }
                var beginElementSetOutcome = BeginElementSetSignal.DispatchResult(key, value);
                if (!beginElementSetOutcome)
                {
                    throw new Exception(beginElementSetOutcome.ErrorMessage);
                }
                var isAdd = !Value.ContainsKey(key);
                Value[key] = value;
                var endElementSetOutcome = EndElementSetSignal.DispatchResult(key, value);
                if (!endElementSetOutcome)
                {
                    throw new Exception(endElementSetOutcome.ErrorMessage);
                }
                if (isAdd)
                {
                    var elementAddedOutcome = ElementAddedSignal.DispatchResult(key, value);
                    if (!elementAddedOutcome)
                    {
                        throw new Exception(elementAddedOutcome.ErrorMessage);
                    }
                    var elementCountChangedOutcome = ElementCountChangedSignal.DispatchResult();
                    if (!elementCountChangedOutcome)
                    {
                        throw new Exception(elementCountChangedOutcome.ErrorMessage);
                    }
                }
            }
        }
        public ISignalOutcome<TKey> ElementGetSignal { get; } = new LazySignalOutcome<TKey>();
        public ISignalOutcome<TKey, TValue> BeginElementSetSignal { get; } = new LazySignalOutcome<TKey, TValue>();
        public ISignalOutcome<TKey, TValue> EndElementSetSignal { get; } = new LazySignalOutcome<TKey, TValue>();
        public ISignalOutcome<TKey, TValue> ElementAddedSignal { get; } = new LazySignalOutcome<TKey, TValue>();
        public ISignalOutcome<TKey, TValue> ElementRemovedSignal { get; } = new LazySignalOutcome<TKey, TValue>();
        public ISignalOutcome ElementCountChangedSignal { get; } = new LazySignalOutcome();

        public Outcome AddElement(TKey key, TValue value)
        {
            var getOutcome = Get(out var dictionary);
            if (!getOutcome)
            {
                return getOutcome;
            }
            if (dictionary!.ContainsKey(key))
            {
                return Outcome.Fail($"Dictionary already contains key {key}");
            }
            var beginElementSetOutcome = BeginElementSetSignal.DispatchResult(key, value);
            if (!beginElementSetOutcome)
            {
                return beginElementSetOutcome;
            }
            dictionary.Add(key, value);
            return
                EndElementSetSignal.DispatchResult(key, value) &&
                ElementAddedSignal.DispatchResult(key, value) &&
                ElementCountChangedSignal.DispatchResult();
        }

        public Outcome RemoveElement(TKey key)
        {
            var getOutcome = Get(out var dictionary);
            if (!getOutcome)
            {
                return getOutcome;
            }
            if (!dictionary!.ContainsKey(key))
            {
                return Outcome.Fail($"Dictionary does not contain key {key}");
            }
            var beginElementSetOutcome = BeginElementSetSignal.DispatchResult(key, default!);
            if (!beginElementSetOutcome)
            {
                return beginElementSetOutcome;
            }
            var value = dictionary[key];
            dictionary.Remove(key);
            return
                EndElementSetSignal.DispatchResult(key, value) &&
                ElementRemovedSignal.DispatchResult(key, value) &&
                ElementCountChangedSignal.DispatchResult();
        }

        public Outcome HasElement(TKey key)
        {
            var getOutcome = Get(out var dictionary);
            if (!getOutcome)
            {
                return getOutcome;
            }
            return dictionary!.ContainsKey(key) ?
                Outcome.Success() :
                Outcome.Fail($"Dictionary does not contain key {key}");
        }

        public Outcome GetElement(TKey key, out TValue? value)
        {
            var getOutcome = Get(out var dictionary);
            if (!getOutcome)
            {
                value = default;
                return getOutcome;
            }
            if (!dictionary!.ContainsKey(key))
            {
                value = default;
                return Outcome.Fail($"Dictionary does not contain key {key}");
            }
            value = dictionary[key];
            return Outcome.Success();
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
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