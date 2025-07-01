using System;
using cpGames.core.RapidIoC;

namespace cpGames.core.EntityComponentFramework.impl
{
    public class ReferenceConverter<TComponent> : IPropertyConverter<TComponent>
        where TComponent : class?, IComponent?
    {
        #region Fields
        private IReferenceResolver? _referenceResolver;
        #endregion

        #region IPropertyConverter<TComponent> Members
        public bool CanConvert(Type type)
        {
            return type == typeof(byte[]) ||
                   type == typeof(string) ||
                   type == typeof(Address);
        }

        public Outcome Convert(object? data, out TComponent? value)
        {
            value = null;

            if (data is byte[] bytes)
            {
                var address = new Address(bytes);
                return GetValueFromAddress(address, out value);
            }
            if (data is string str)
            {
                var address = new Address(str);
                return GetValueFromAddress(address, out value);
            }
            if (data is Address)
            {
                var address = (Address)data;
                return GetValueFromAddress(address, out value);
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

        private Outcome GetValueFromAddress(Address address, out TComponent? value)
        {
            value = null;
            if (address == Address.INVALID)
            {
                return Outcome.Success();
            }
            IComponent? component = default;
            var getEntityOutcome =
                GetReferenceResolver() &&
                _referenceResolver!.ResolveReference(address, out component);
            if (!getEntityOutcome)
            {
                return getEntityOutcome;
            }
            if (component is not TComponent)
            {
                return Outcome.Fail($"Component {component!.GetType()} is not of type {typeof(TComponent)}");
            }
            value = (TComponent)component;
            return Outcome.Success();
        }

        public Outcome ConvertReferenceToAddress(IComponent component, out Address address)
        {
            address = Address.INVALID;
            var outcome =
                GetReferenceResolver() &&
                _referenceResolver!.ConvertReferenceToAddress(component, out address);
            if (!outcome)
            {
                address = Address.INVALID;
                return outcome;
            }
            return Outcome.Success();
        }
        #endregion
    }
}