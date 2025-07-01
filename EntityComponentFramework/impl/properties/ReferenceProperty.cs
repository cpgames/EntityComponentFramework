using System;
using Newtonsoft.Json;

namespace cpGames.core.EntityComponentFramework.impl
{
    public class ReferencePropertyJsonConverter : JsonConverter
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

    [JsonConverter(typeof(ReferencePropertyJsonConverter))]
    public class ReferenceProperty<TComponent> : Property<TComponent?>, IReferenceProperty<TComponent>
        where TComponent : class, IComponent
    {
        #region Fields
        private readonly ReferenceConverter<TComponent?> _referenceConverter = new();
        #endregion

        #region Constructors
        public ReferenceProperty(Entity owner, string name) : base(owner, name, null)
        {
            _converters.Add(_referenceConverter);
        }
        #endregion

        #region IReferenceProperty<TComponent> Members
        public override Outcome Connect()
        {
            return
                base.Connect() &&
                BeginValueSetSignal.AddCommand(OnBeginValueSet, this) &&
                EndValueSetSignal.AddCommand(OnEndValueSet, this);
        }

        public override Outcome Disconnect()
        {
            return
                BeginValueSetSignal.RemoveCommand(this) &&
                EndValueSetSignal.RemoveCommand(this) &&
                base.Disconnect();
        }

        public Outcome GetOtherComponent<TOtherComponent>(out TOtherComponent? otherComponent) where TOtherComponent : class, IComponent
        {
            otherComponent = null;
            return
                GetNonDefault(out var component) &&
                component!.Entity.GetComponent(out otherComponent);
        }

        public Outcome GetDerivedNonDefault<TDerivedComponent>(out TDerivedComponent? derivedComponent) where TDerivedComponent : class, TComponent
        {
            var getBaseComponent = GetNonDefault(out var baseComponent);
            if (!getBaseComponent)
            {
                derivedComponent = null;
                return getBaseComponent;
            }
            if (baseComponent is not TDerivedComponent value)
            {
                derivedComponent = null;
                return Outcome.Fail($"Component {baseComponent!.GetType()} is not of type {typeof(TDerivedComponent)}");
            }
            derivedComponent = value;
            return Outcome.Success();
        }
        #endregion

        #region Methods
        protected override Outcome ConvertToData(TComponent? value, out object? data)
        {
            if (value == null)
            {
                data = Address.INVALID;
                return Outcome.Success();
            }
            var getAddressOutcome = _referenceConverter.ConvertReferenceToAddress(value, out var address);
            if (!getAddressOutcome)
            {
                data = Address.INVALID;
                return getAddressOutcome;
            }
            data = address;
            return Outcome.Success();
        }

        private Outcome OnBeginValueSet(object? value)
        {
            if (_value != null)
            {
                return _value.BeginDisconnectedSignal.RemoveCommand(this);
            }
            return Outcome.Success();
        }

        private Outcome OnEndValueSet(object? value)
        {
            if (value is TComponent component)
            {
                return component.BeginDisconnectedSignal.AddCommand(OnReferencedComponentBeginDisconnected, this);
            }
            return Outcome.Success();
        }

        private Outcome OnReferencedComponentBeginDisconnected(object? value)
        {
            return ResetToDefault();
        }
        #endregion
    }
}