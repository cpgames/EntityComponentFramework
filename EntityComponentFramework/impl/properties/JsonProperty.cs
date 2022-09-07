using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace cpGames.core.EntityComponentFramework.impl
{
    public class JsonPropertyConverter<TModel> : JsonConverter
        where TModel : class
    {
        #region Methods
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var property = (JsonProperty<TModel>)value;
            writer.WriteRawValue((string)property.Data);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (!(existingValue is JsonProperty<TModel> property))
            {
                throw new Exception("No existing value");
            }
            var jsonObject = JObject.Load(reader);
            var model = jsonObject.ToObject<TModel>(serializer);
            var setOutcome = property.Set(model);
            if (!setOutcome)
            {
                throw new Exception(setOutcome.ErrorMessage);
            }
            return existingValue;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(JsonProperty<TModel>).IsAssignableFrom(objectType);
        }
        #endregion
    }

    public abstract class JsonProperty<TModel> : Property<TModel>, IJsonProperty<TModel>
        where TModel : class
    {
        #region Nested type: JsonComparer
        private class JsonComparer : EqualityComparer<TModel>
        {
            #region Methods
            public override bool Equals(TModel x, TModel y)
            {
                if (ReferenceEquals(x, y))
                {
                    return true;
                }
                var jsonX = JsonConvert.SerializeObject(x);
                var jsonY = JsonConvert.SerializeObject(y);
                return jsonX == jsonY;
            }

            public override int GetHashCode(TModel obj)
            {
                return obj.GetHashCode();
            }
            #endregion
        }
        #endregion

        #region Fields
        private string _jsonString;
        #endregion

        #region Properties
        protected override EqualityComparer<TModel> ValueComparer { get; } = new JsonComparer();
        #endregion

        #region IJsonProperty<TModel> Members
        public override object Data => _jsonString;

        public Outcome Clone(out TModel value)
        {
            return Convert(Data, out value);
        }
        #endregion

        #region Methods
        protected override Outcome Convert(object data, out TModel value)
        {
            if (data == null)
            {
                value = default;
                return Outcome.Success();
            }
            value = (TModel)JsonConvert.DeserializeObject((string)data, typeof(TModel),
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All,
                    MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead
                });
            if (value == null)
            {
                return Outcome.Fail($"Failed to convert <{data.GetType().Name}> to <{typeof(TModel).Name}>.");
            }
            return Outcome.Success();
        }

        protected override Outcome UpdateValue(TModel value)
        {
            try
            {
                _jsonString = JsonConvert.SerializeObject(value, Formatting.Indented,
                    new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
            }
            catch (Exception e)
            {
                return Outcome.Fail(e.Message);
            }
            _value = value;
            return Outcome.Success();
        }
        #endregion
    }
}