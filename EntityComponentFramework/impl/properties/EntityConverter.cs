using System;
using cpGames.core.RapidIoC;

namespace cpGames.core.EntityComponentFramework.impl
{
    public class EntityConverter : IPropertyConverter<Entity?>
    {
        #region Fields
        private IReferenceResolver? _referenceResolver;
        #endregion

        #region IPropertyConverter<Entity?> Members
        public bool CanConvert(Type type)
        {
            return type == typeof(byte[]) ||
                   type == typeof(string) ||
                   type == typeof(Id);
        }

        public Outcome Convert(object? data, out Entity? value)
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

        private Outcome GetEntityFromId(Id id, out Entity? value)
        {
            value = null;
            if (!id.IsValid)
            {
                return Outcome.Success();
            }
            return
                GetReferenceResolver() &&
                _referenceResolver!.ResolveEntity(id, out value);
        }
        #endregion
    }
}
