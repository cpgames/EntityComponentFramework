using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace cpGames.core.EntityComponentFramework.impl
{
    public class JsonPropertyConverter<TModel> : JsonConverter
        where TModel : class
    {
        #region Methods
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            var property = (JsonProperty<TModel>)value;
            if (!property.GetData(out var data))
            {
                writer.WriteNull();
                return;
            }
            writer.WriteRawValue((string)data!);
        }

        public override object? ReadJson(
            JsonReader reader,
            Type objectType,
            object? existingValue,
            JsonSerializer serializer)
        {
            if (existingValue is not JsonProperty<TModel> property)
            {
                return null;
            }
            var jsonObject = JObject.Load(reader);
            var model = jsonObject.ToObject<TModel>(serializer);
            var setOutcome = property.Set(model);
            return !setOutcome ? null : existingValue;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(JsonProperty<TModel>).IsAssignableFrom(objectType);
        }
        #endregion
    }

    public abstract class JsonProperty<TModel> : Property<TModel?>, IJsonProperty<TModel>
        where TModel : class
    {
        #region Nested type: JsonComparer
        private class JsonComparer : EqualityComparer<TModel?>
        {
            #region Methods
            public override bool Equals(TModel? x, TModel? y)
            {
                if (ReferenceEquals(x, y))
                {
                    return true;
                }
                var jsonX = JsonConvert.SerializeObject(x);
                var jsonY = JsonConvert.SerializeObject(y);
                return jsonX == jsonY;
            }

            public override int GetHashCode(TModel? obj)
            {
                return obj != null ? obj.GetHashCode() : 0;
            }
            #endregion
        }
        #endregion

        #region Fields
        private string _jsonString = string.Empty;
        #endregion

        #region Properties
        protected override EqualityComparer<TModel?> ValueComparer { get; } = new JsonComparer();
        #endregion

        #region Constructors
        protected JsonProperty(Entity owner, string name, TModel defaultValue) : base(owner, name, defaultValue) { }
        protected JsonProperty(Entity owner, string name) : base(owner, name, null) { }
        #endregion

        #region IJsonProperty<TModel> Members
        public Outcome Clone(out TModel? value)
        {
            value = default;
            return
                GetData(out var data) &&
                ConvertToValue(data, out value);
        }

        public Outcome RefreshJson()
        {
            if (_value == null)
            {
                return Outcome.Success();
            }
            try
            {
                _jsonString = JsonConvert.SerializeObject(_value);
            }
            catch (Exception e)
            {
                return Outcome.Fail(e.Message);
            }
            return Outcome.Success();
        }
        #endregion

        #region Methods
        protected override Outcome ConvertToValue(object? data, out TModel? value)
        {
            value = default;
            if (data == null)
            {
                return Outcome.Success();
            }
            object? valueObj;
            try
            {
                string strData;
                if (data is byte[] bytes)
                {
                    strData = Encoding.UTF8.GetString(bytes);
                }
                else
                {
                    strData = (string)data;
                }
                valueObj = JsonConvert.DeserializeObject(strData, typeof(TModel));
            }
            catch (Exception e)
            {
                return Outcome.Fail(e.Message);
            }
            if (valueObj != null)
            {
                value = (TModel)valueObj;
            }
            return value == null ?
                Outcome.Fail($"Failed to convert <{data.GetType().Name}> to <{typeof(TModel).Name}>.") :
                Outcome.Success();
        }

        protected override Outcome ConvertToData(TModel? value, out object? data)
        {
            data = _jsonString;
            return Outcome.Success();
        }

        protected override Outcome UpdateValue(TModel? value)
        {
            try
            {
                _jsonString = JsonConvert.SerializeObject(value);
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