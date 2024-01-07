using System;
using Newtonsoft.Json;

namespace cpGames.core.EntityComponentFramework.impl
{
    public class StringPropertyConverter : JsonConverter
    {
        #region Methods
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value == null ||
                !((StringProperty)value).Get(out var str))
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
            if (existingValue is not StringProperty property)
            {
                throw new Exception("No existing value");
            }
            if (reader.Value == null)
            {
                property.Set(string.Empty);
                return existingValue;
            }
            var setOutcome = property.Set((string)reader.Value);
            if (!setOutcome)
            {
                throw new Exception(setOutcome.ErrorMessage);
            }
            return existingValue;
        }

        public override bool CanConvert(Type objectType)
        {
            var isAssignable = objectType.IsAssignableFrom(typeof(StringProperty));
            return isAssignable;
        }
        #endregion
    }

    [JsonConverter(typeof(StringPropertyConverter))]
    public class StringProperty : Property<string>, IStringProperty
    {
        #region Constructors
        public StringProperty(Entity owner, string name, string defaultValue) : base(owner, name, defaultValue) { }
        public StringProperty(Entity owner, string name) : base(owner, name, string.Empty) { }
        #endregion

        #region Methods
        protected override Outcome ConvertToValue(object? data, out string? value)
        {
            if (data == null)
            {
                value = string.Empty;
                return Outcome.Success();
            }
            return base.ConvertToValue(data, out value);
        }
        #endregion
    }
}