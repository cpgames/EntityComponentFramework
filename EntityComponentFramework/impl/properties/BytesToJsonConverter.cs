using System;
using System.Text;
using Newtonsoft.Json;

namespace cpGames.core.EntityComponentFramework.impl
{
    public class BytesToJsonConverter : IConverter
    {
        public bool CanConvert(Type sourceType, Type targetType)
        {
            return sourceType == typeof(byte[]) && targetType.IsClass;
        }

        public Outcome Convert(object? data, Type targetType, out object? value)
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
                    if (strData.Length > 0 && strData[0] == '\uFEFF')
                    {
                        strData = strData.Substring(1);
                    }
                    value = JsonConvert.DeserializeObject(strData, targetType);
                }
                catch (Exception e)
                {
                    value = default;
                    return Outcome.Fail(e.Message);
                }
                return Outcome.Success();
            }

            return Outcome.Fail($"Cannot convert {data?.GetType().Name} to {targetType.Name}");
        }
    }
}