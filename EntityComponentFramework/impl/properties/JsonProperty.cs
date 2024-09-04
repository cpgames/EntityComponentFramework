using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace cpGames.core.EntityComponentFramework.impl
{
    public class JsonPropertyConverter<TModel> : JsonConverter
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
            return !setOutcome ?
                null :
                existingValue;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(JsonProperty<TModel>).IsAssignableFrom(objectType);
        }
        #endregion
    }

    public abstract class JsonProperty<TModel> : Property<TModel?>, IJsonProperty<TModel>
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
                return obj != null ?
                    obj.GetHashCode() :
                    0;
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
                return Outcome.Fail(e.Message, this);
            }
            return Outcome.Success();
        }
        #endregion

        #region Methods
        protected override Outcome ConvertToValue(object? data, out TModel? value)
        {
            if (data is string strData)
            {
                if (string.IsNullOrEmpty(strData))
                {
                    value = default;
                    return Outcome.Success();
                }
                try
                {
                    var valueObj = JsonConvert.DeserializeObject(strData, typeof(TModel))!;
                    value = (TModel)valueObj;
                }
                catch (Exception e)
                {
                    value = default;
                    return Outcome.Fail(e.Message, this);
                }
                return Outcome.Success();
            }
            if (data is byte[] bytes)
            {
                if (bytes.Length == 0)
                {
                    value = default;
                    return Outcome.Success();
                }
                try
                {
                    strData = Encoding.UTF8.GetString(bytes);
                    var valueObj = JsonConvert.DeserializeObject(strData, typeof(TModel))!;
                    value = (TModel)valueObj;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                return Outcome.Success();
            }
            return base.ConvertToValue(data, out value);
        }

        protected override Outcome ConvertToData(TModel? value, out object? data)
        {
            if (string.IsNullOrEmpty(_jsonString) && value != null)
            {
                var refreshOutcome = RefreshJson();
                if (!refreshOutcome)
                {
                    data = null;
                    return refreshOutcome;
                }
            }
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
                return Outcome.Fail(e.Message, this);
            }
            _value = value;
            return Outcome.Success();
        }
        #endregion
    }
}