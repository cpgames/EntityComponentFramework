using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace cpGames.core.EntityComponentFramework.impl
{
    public class JsonPropertyConverter<TValue> : JsonConverter
    {
        #region Methods
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            var property = (JsonProperty<TValue>)value;
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
            if (existingValue is not JsonProperty<TValue> property)
            {
                return null;
            }
            var jsonObject = JObject.Load(reader);
            var model = jsonObject.ToObject<TValue>(serializer);
            var setOutcome = property.Set(model);
            return !setOutcome ?
                null :
                existingValue;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(JsonProperty<TValue>).IsAssignableFrom(objectType);
        }
        #endregion
    }

    public abstract class JsonProperty<TValue> : Property<TValue?>, IJsonProperty<TValue>
    {
        #region Nested type: JsonComparer
        private class JsonComparer : EqualityComparer<TValue?>
        {
            #region Methods
            public override bool Equals(TValue? x, TValue? y)
            {
                if (ReferenceEquals(x, y))
                {
                    return true;
                }
                var jsonX = JsonConvert.SerializeObject(x);
                var jsonY = JsonConvert.SerializeObject(y);
                return jsonX == jsonY;
            }

            public override int GetHashCode(TValue? obj)
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
        protected override EqualityComparer<TValue?> ValueComparer { get; } = new JsonComparer();
        #endregion

        #region Constructors
        protected JsonProperty(Entity owner, string name, TValue defaultValue) : base(owner, name, defaultValue) { }
        #endregion

        #region IJsonProperty<TValue> Members
        public Outcome Clone(out TValue? value)
        {
            value = default;
            return
                GetData(out var data) &&
                ConvertToValue(data, out value);
        }

        public Outcome GetDerivedNonDefault<TDerivedModel>(out TDerivedModel? derivedModel) where TDerivedModel : class, TValue
        {
            derivedModel = default;
            var outcome = GetNonDefault(out var model);
            if (!outcome)
            {
                return outcome;
            }
            if (model is not TDerivedModel derived)
            {
                return Outcome.Fail($"Model is not of type {typeof(TDerivedModel).Name}");
            }
            derivedModel = derived;
            return Outcome.Success();
        }

        public Outcome RefreshJson()
        {
            if (_value == null)
            {
                return Outcome.Success();
            }
            try
            {
                _jsonString = JsonConvert.SerializeObject(_value, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
            }
            catch (Exception e)
            {
                return Outcome.Fail(e.Message);
            }
            return Outcome.Success();
        }
        #endregion

        #region Methods
        protected override Outcome ConvertToValue(object? data, out TValue? value)
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
                    var valueObj = JsonConvert.DeserializeObject(strData, typeof(TValue))!;
                    value = (TValue)valueObj;
                }
                catch (Exception e)
                {
                    value = default;
                    return Outcome.Fail(e.Message);
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
                    var valueObj = JsonConvert.DeserializeObject(strData, typeof(TValue))!;
                    value = (TValue)valueObj;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                return Outcome.Success();
            }
            if (data is TValue newValue)
            {
                value = Clone(newValue);
                return Outcome.Success();
            }
            return base.ConvertToValue(data, out value);
        }

        protected override Outcome ConvertToData(TValue? value, out object? data)
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

        protected override Outcome UpdateValue(TValue? value)
        {
            try
            {
                _jsonString = JsonConvert.SerializeObject(value, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
            }
            catch (Exception e)
            {
                return Outcome.Fail(e.Message);
            }
            _value = value;
            return Outcome.Success();
        }

        protected override TValue? Clone(TValue? value)
        {
            if (value == null)
            {
                return default;
            }
            var json = JsonConvert.SerializeObject(value, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
            return JsonConvert.DeserializeObject<TValue>(json);
        }
        #endregion
    }
}