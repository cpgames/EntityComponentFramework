using System;
using System.Text;
using Newtonsoft.Json;

namespace cpGames.core.EntityComponentFramework.impl
{
    public class BytesToJsonConverter<TValue> : IPropertyConverter<TValue?>
    {
        public bool CanConvert(Type type)
        {
            return type == typeof(byte[]);
        }

        public Outcome Convert(object? data, out TValue? value)
        {
            value = default;

            if (data is byte[] bytes)
            {
                if (bytes.Length == 0)
                {
                    value = default;
                    return Outcome.Success();
                }
                try
                {
                    var strData = Encoding.UTF8.GetString(bytes);
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