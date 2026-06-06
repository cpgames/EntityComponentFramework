using System;
using Newtonsoft.Json;

namespace cpGames.core.EntityComponentFramework.impl
{
    public class EntityReferencePropertyJsonConverter : JsonConverter
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
            return typeof(IEntityReferenceProperty).IsAssignableFrom(objectType);
        }
        #endregion
    }

    [JsonConverter(typeof(EntityReferencePropertyJsonConverter))]
    public class EntityReferenceProperty : Property<Entity?>, IEntityReferenceProperty
    {
        #region Fields
        private readonly EntityConverter _entityConverter = new();
        #endregion

        #region Constructors
        public EntityReferenceProperty(Entity owner, string name) : base(owner, name, null)
        {
            _converters.Add(_entityConverter);
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
