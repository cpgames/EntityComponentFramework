using System;
using Newtonsoft.Json;

namespace cpGames.core.EntityComponentFramework.impl
{
    public class FloatPropertyConverter : JsonConverter
    {
        #region Methods
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value == null ||
                !((FloatProperty)value).Get(out var val))
            {
                writer.WriteNull();
                return;
            }
            writer.WriteValue(val);
        }

        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object? existingValue,
            JsonSerializer serializer)
        {
            if (existingValue is not FloatProperty property)
            {
                throw new Exception("No existing value");
            }
            if (reader.Value == null)
            {
                property.Set(0);
                return existingValue;
            }
            var setOutcome = property.SetData(reader.Value);
            if (!setOutcome)
            {
                throw new Exception(setOutcome.ErrorMessage);
            }
            return existingValue;
        }

        public override bool CanConvert(Type objectType)
        {
            var isAssignable = objectType.IsAssignableFrom(typeof(FloatProperty));
            return isAssignable;
        }
        #endregion
    }

    [JsonConverter(typeof(FloatPropertyConverter))]
    public class FloatProperty : Property<float>, IFloatProperty
    {
        #region Constructors
        public FloatProperty(Entity owner, string name) : base(owner, name, 0f) { }
        #endregion

        #region IFloatProperty Members
        public Outcome Add(float value)
        {
            var outcome =
                Get(out var currentValue) &&
                Set(currentValue + value);
            return outcome;
        }

        public Outcome Subtract(float value)
        {
            var outcome =
                Get(out var currentValue) &&
                Set(currentValue - value);
            return outcome;
        }

        public Outcome Min(float value)
        {
            var outcome =
                Get(out var currentValue) &&
                Set(Math.Min(currentValue, value));
            return outcome;
        }
        public Outcome Max(float value)
        {
            var outcome =
                Get(out var currentValue) &&
                Set(Math.Max(currentValue, value));
            return outcome;
        }
        #endregion

        #region Methods
        protected override Outcome ConvertToValue(object? data, out float value)
        {
            if (data is double d)
            {
                value = (float)d;
                return Outcome.Success();
            }
            return base.ConvertToValue(data, out value);
        }
        #endregion
    }
}