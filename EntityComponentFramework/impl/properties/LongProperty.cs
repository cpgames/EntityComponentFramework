using System;
using Newtonsoft.Json;

namespace cpGames.core.EntityComponentFramework.impl
{
    public class LongPropertyConverter : JsonConverter
    {
        #region Methods
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value == null ||
                !((LongProperty)value).Get(out var val))
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
            if (existingValue is not LongProperty property)
            {
                throw new Exception("No existing value");
            }
            if (reader.Value == null)
            {
                property.Set(0);
                return existingValue;
            }
            var longValue = Convert.ToInt64(reader.Value);
            var setOutcome = property.SetData(longValue);
            if (!setOutcome)
            {
                throw new Exception(setOutcome.ErrorMessage);
            }
            return existingValue;
        }

        public override bool CanConvert(Type objectType)
        {
            var isAssignable = objectType.IsAssignableFrom(typeof(LongProperty));
            return isAssignable;
        }
        #endregion
    }

    [JsonConverter(typeof(LongPropertyConverter))]
    public class LongProperty : Property<long>, ILongProperty
    {
        #region Constructors
        public LongProperty(Entity owner, string name) : base(owner, name, 0) { }
        #endregion

        #region ILongProperty Members
        public Outcome Add(long value)
        {
            return
                Get(out var currentValue) &&
                Set(currentValue + value);
        }

        public Outcome Subtract(long value)
        {
            return
                Get(out var currentValue) &&
                Set(currentValue - value);
        }
        #endregion
    }
}