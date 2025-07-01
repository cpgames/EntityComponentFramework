using System;
using System.Collections;
using System.Collections.Generic;

namespace cpGames.core.EntityComponentFramework.impl
{
    public class ListConverter<TElementValue> : IPropertyConverter<List<TElementValue>>
    {
        #region IPropertyConverter<List<TElementValue>> Members
        public bool CanConvert(Type type)
        {
            return typeof(IList).IsAssignableFrom(type) ||
                   typeof(IEnumerable).IsAssignableFrom(type) ||
                   type.IsArray;
        }

        public Outcome Convert(object? data, out List<TElementValue>? value)
        {
            value = null;

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

            if (data is IEnumerable enumerable && !(data is IList))
            {
                value = new List<TElementValue>();
                foreach (var entry in enumerable)
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

            if (data is Array array)
            {
                value = new List<TElementValue>();
                foreach (var entry in array)
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

            return Outcome.Fail($"Cannot convert {data?.GetType().Name} to List<{typeof(TElementValue).Name}>");
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
            return Outcome.Fail($"Cannot convert entry {data} to {typeof(TElementValue)}");
        }
        #endregion
    }
} 