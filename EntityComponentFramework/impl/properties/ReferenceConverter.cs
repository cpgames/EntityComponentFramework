using System;
using cpGames.core.RapidIoC;

namespace cpGames.core.EntityComponentFramework.impl
{
    public class ReferenceConverter<TComponent> : IPropertyConverter<TComponent?>
        where TComponent : class, IComponent
    {
        #region Fields
        private IReferenceResolver? _referenceResolver;
        #endregion

        #region IPropertyConverter<TComponent?> Members
        public bool CanConvert(Type type)
        {
            return type == typeof(byte[]) ||
                   type == typeof(string) ||
                   type == typeof(Id);
        }

        public Outcome Convert(object? data, out TComponent? value)
        {
            value = null;

            if (data is byte[] bytes)
            {
                return GetValueFromId(new Id(bytes), out value);
            }
            if (data is string str)
            {
                return GetValueFromId(new Id(str), out value);
            }
            if (data is Id id)
            {
                return GetValueFromId(id, out value);
            }

            return Outcome.Fail($"Cannot convert {data?.GetType().Name} to {typeof(TComponent).Name}");
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

        private Outcome GetValueFromId(Id id, out TComponent? value)
        {
            value = null;
            if (!id.IsValid)
            {
                return Outcome.Success();
            }
            return
                GetReferenceResolver() &&
                _referenceResolver!.ResolveReference(id, out value);
        }

        public Outcome ConvertReferenceToId(IComponent component, out Id id)
        {
            id = Id.INVALID;
            return
                GetReferenceResolver() &&
                _referenceResolver!.ConvertReferenceToId(component, out id);
        }
        #endregion
    }
}
