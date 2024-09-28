using System;
using Newtonsoft.Json;

namespace cpGames.core.EntityComponentFramework.impl
{
    public class IntPropertyConverter : JsonConverter
    {
        #region Methods
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value == null ||
                !((IntProperty)value).Get(out var val))
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
            if (existingValue is not IntProperty property)
            {
                throw new Exception("No existing value");
            }
            if (reader.Value == null)
            {
                property.Set(0);
                return existingValue;
            }
            var intValue = Convert.ToInt32(reader.Value);
            var setOutcome = property.SetData(intValue);
            if (!setOutcome)
            {
                throw new Exception(setOutcome.ErrorMessage);
            }
            return existingValue;
        }

        public override bool CanConvert(Type objectType)
        {
            var isAssignable = objectType.IsAssignableFrom(typeof(IntProperty));
            return isAssignable;
        }
        #endregion
    }

    [JsonConverter(typeof(IntPropertyConverter))]
    public class IntProperty : Property<int>, IIntProperty
    {
        #region Constructors
        public IntProperty(Entity owner, string name) : base(owner, name, 0) { }
        #endregion

        #region IIntProperty Members
        public Outcome Add(int value)
        {
            return
                Get(out var currentValue) &&
                Set(currentValue + value);
        }

        public Outcome Subtract(int value)
        {
            return
                Get(out var currentValue) &&
                Set(currentValue - value);
        }
        #endregion
    }
}