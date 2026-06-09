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
                !((IProperty)value).GetData(out var data))
            {
                writer.WriteNull();
                return;
            }
            writer.WriteValue(data!.ToString());
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
                property.SetData(Id.INVALID);
                return existingValue;
            }
            var outcome = property.SetData((string)reader.Value);
            if (!outcome)
            {
                throw new Exception(outcome.ErrorMessage);
            }
            return existingValue;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(IReferenceProperty).IsAssignableFrom(objectType);
        }
        #endregion
    }

    [JsonConverter(typeof(ReferencePropertyJsonConverter))]
    public class ReferenceProperty : Property<Entity?>, IReferenceProperty
    {
        #region Constructors
        public ReferenceProperty(Entity owner, string name) : base(owner, name, null)
        {
        }
        #endregion

        #region Methods
        protected override Outcome ConvertToData(Entity? value, out object? data)
        {
            data = value?.Id ?? Id.INVALID;
            return Outcome.Success();
        }
        #endregion
    }
}
