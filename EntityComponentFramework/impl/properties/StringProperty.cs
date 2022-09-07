using System;
using Newtonsoft.Json;

namespace cpGames.core.EntityComponentFramework.impl
{
    public class StringPropertyConverter : JsonConverter
    {
        #region Methods
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((StringProperty)value).Value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (!(existingValue is StringProperty property))
            {
                throw new Exception("No existing value");
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
            return typeof(StringProperty).IsAssignableFrom(objectType);
        }
        #endregion
    }

    [JsonConverter(typeof(StringPropertyConverter))]
    public class StringProperty : Property<string>, IStringProperty { }
}