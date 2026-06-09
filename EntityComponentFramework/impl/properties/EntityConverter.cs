using System;
using cpGames.core.RapidIoC;

namespace cpGames.core.EntityComponentFramework.impl
{
    public class EntityConverter : IConverter
    {
        #region Fields
        private IReferenceResolver? _referenceResolver;
        #endregion

        #region IConverter Members
        public bool CanConvert(Type sourceType, Type targetType)
        {
            return (sourceType == typeof(byte[]) ||
                    sourceType == typeof(string) ||
                    sourceType == typeof(Id)) &&
                   targetType == typeof(Entity);
        }

        public Outcome Convert(object? data, Type targetType, out object? value)
        {
            value = null;

            if (data is byte[] bytes)
            {
                return GetEntityFromId(new Id(bytes), out value);
            }
            if (data is string str)
            {
                return GetEntityFromId(new Id(str), out value);
            }
            if (data is Id id)
            {
                return GetEntityFromId(id, out value);
            }

            return Outcome.Fail($"Cannot convert {data?.GetType().Name} to Entity");
        }
        #endregion

        #region Methods
        private Outcome GetReferenceResolver()
        {
            return
                _referenceResolver != null ?
                    Outcome.Success() :
                    Rapid.GetBindingValue(Rapid.RootKey, out _referenceResolver);
        }

        private Outcome GetEntityFromId(Id id, out object? value)
        {
            value = null;
            if (!id.IsValid)
            {
                return Outcome.Success();
            }
            var outcome = GetReferenceResolver();
            if (!outcome)
            {
                return outcome;
            }
            outcome = _referenceResolver!.ResolveEntity(id, out var entity);
            value = entity;
            return outcome;
        }
        #endregion
    }
}
