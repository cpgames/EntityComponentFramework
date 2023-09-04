using System;
using Newtonsoft.Json;

namespace cpGames.core.EntityComponentFramework.impl
{
    public class ReferencePropertyConverter : JsonConverter
    {
        #region Methods
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value == null ||
                !((IProperty)value).GetData(out var str))
            {
                writer.WriteNull();
                return;
            }
            writer.WriteValue(str);
        }

        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object? existingValue,
            JsonSerializer serializer)
        {
            if (existingValue is not IProperty property)
            {
                throw new Exception("No existing value");
            }
            if (reader.Value == null)
            {
                property.SetData(Address.INVALID);
                return existingValue;
            }
            var setOutcome = property.SetData((string)reader.Value);
            if (!setOutcome)
            {
                throw new Exception(setOutcome.ErrorMessage);
            }
            return existingValue;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.IsAssignableFrom(typeof(StringProperty));
        }
        #endregion
    }

    [JsonConverter(typeof(ReferencePropertyConverter))]
    public class ReferenceProperty<TComponent> : Property<TComponent?>, IReferenceProperty<TComponent>
        where TComponent : class, IComponent
    {
        #region Constructors
        public ReferenceProperty(Entity owner, string name) : base(owner, name, null) { }
        #endregion

        #region IReferenceProperty<TComponent> Members
        public Outcome GetOtherComponent<TOtherComponent>(out TOtherComponent? otherComponent) where TOtherComponent : class, IComponent
        {
            otherComponent = null;
            return
                Get(out var component) &&
                component!.Entity.GetComponent(out otherComponent);
        }

        public Outcome HasTarget()
        {
            var getOutcome = Get(out var value);
            if (!getOutcome)
            {
                return getOutcome;
            }
            return value != null ?
                Outcome.Success() :
                Outcome.Fail($"Reference {Name} is not established.");
        }
        #endregion

        #region Methods
        protected override Outcome ConvertToValue(object? data, out TComponent? value)
        {
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
            return base.ConvertToValue(data, out value);
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
                Owner.GetComponent<IReferenceResolverComponent>(out var referenceResolver) &&
                referenceResolver!.ResolveReference(address, out component);
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

        protected override Outcome ConvertToData(TComponent? value, out object? data)
        {
            if (value == null)
            {
                data = Address.INVALID;
                return Outcome.Success();
            }
            Address address = Address.INVALID;
            var getAddressOutcome =
                Owner.GetComponent<IReferenceResolverComponent>(out var referenceResolver) &&
                referenceResolver!.ConvertReferenceToAddress(value, out address);
            if (!getAddressOutcome)
            {
                data = null;
                return getAddressOutcome;
            }
            data = address.ToString();
            return Outcome.Success();
        }
        #endregion
    }
}