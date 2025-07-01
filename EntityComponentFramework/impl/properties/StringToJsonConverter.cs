using System;
using Newtonsoft.Json;

namespace cpGames.core.EntityComponentFramework.impl
{
    public class StringToJsonConverter<TValue> : IPropertyConverter<TValue?>
    {
        public bool CanConvert(Type type)
        {
            return type == typeof(string);
        }

        public Outcome Convert(object? data, out TValue? value)
        {
            value = default;

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

            return Outcome.Fail($"Cannot convert {data?.GetType().Name} to {typeof(TValue).Name}");
        }
    }
} 