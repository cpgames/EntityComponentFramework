using System;
using Newtonsoft.Json;

namespace cpGames.core.EntityComponentFramework.impl
{
    public class BoolPropertyConverter : JsonConverter
    {
        #region Methods
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value == null ||
                !((BoolProperty)value).Get(out var val))
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
            if (existingValue is not BoolProperty property)
            {
                throw new Exception("No existing value");
            }
            if (reader.Value == null)
            {
                property.Set(false);
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
            var isAssignable = objectType.IsAssignableFrom(typeof(BoolProperty));
            return isAssignable;
        }
        #endregion
    }

    [JsonConverter(typeof(BoolPropertyConverter))]
    public class BoolProperty : Property<bool>, IBoolProperty
    {
        #region Constructors
        public BoolProperty(Entity owner, string name) : base(owner, name, false)
        {
            _converters.Add(new IntToBoolConverter());
            _converters.Add(new LongToBoolConverter());
            _converters.Add(new ByteToBoolConverter());
        }
        #endregion

        #region Methods
        protected override Outcome ConvertToData(bool value, out object? data)
        {
            data = value ?
                1 :
                0;
            return Outcome.Success();
        }
        #endregion
    }
}