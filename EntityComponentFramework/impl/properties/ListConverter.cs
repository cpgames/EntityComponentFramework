using System;
using System.Collections;

namespace cpGames.core.EntityComponentFramework.impl
{
    public class ListConverter : IConverter
    {
        #region IConverter Members
        public bool CanConvert(Type sourceType, Type targetType)
        {
            return (typeof(IList).IsAssignableFrom(sourceType) ||
                    typeof(IEnumerable).IsAssignableFrom(sourceType) ||
                    sourceType.IsArray) &&
                   targetType.IsGenericType &&
                   targetType.GetGenericTypeDefinition() == typeof(System.Collections.Generic.List<>);
        }

        public Outcome Convert(object? data, Type targetType, out object? value)
        {
            value = null;

            var elementType = targetType.IsGenericType ?
                targetType.GetGenericArguments()[0] :
                typeof(object);

            if (data is IList list)
            {
                return BuildList(list, targetType, elementType, out value);
            }

            if (data is IEnumerable enumerable)
            {
                return BuildList(enumerable, targetType, elementType, out value);
            }

            if (data is Array array)
            {
                return BuildList(array, targetType, elementType, out value);
            }

            return Outcome.Fail($"Cannot convert {data?.GetType().Name} to {targetType.Name}");
        }
        #endregion

        #region Methods
        private Outcome BuildList(IEnumerable source, Type targetType, Type elementType, out object? value)
        {
            var list = (IList)Activator.CreateInstance(targetType)!;
            foreach (var entry in source)
            {
                var outcome = ConvertEntry(entry, elementType, out var entryValue);
                if (!outcome)
                {
                    value = null;
                    return outcome;
                }
                list.Add(entryValue);
            }
            value = list;
            return Outcome.Success();
        }

        protected virtual Outcome ConvertEntry(object? data, Type elementType, out object? value)
        {
            if (data != null && elementType.IsInstanceOfType(data))
            {
                value = data;
                return Outcome.Success();
            }
            value = null;
            return Outcome.Fail($"Cannot convert entry {data} to {elementType}");
        }
        #endregion
    }
}