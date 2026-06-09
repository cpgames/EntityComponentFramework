using System;
using System.Collections.Generic;
using System.Linq;

namespace cpGames.core.EntityComponentFramework.impl
{
    public static class PropertyConverters
    {
        #region Fields
        private static readonly List<IConverter> _converters = new();
        #endregion

        #region Constructors
        static PropertyConverters()
        {
            Register(new DoubleToFloatConverter());
            Register(new IntToFloatConverter());
            Register(new LongToFloatConverter());
            Register(new IntToBoolConverter());
            Register(new LongToBoolConverter());
            Register(new ByteToBoolConverter());
            Register(new LongToIntConverter());
            Register(new StringToIdConverter());
            Register(new ReferenceToIdConverter());
            Register(new StringToByteArrayConverter());
            Register(new StringToDateTimeConverter());
            Register(new StringToAddressConverter());
            Register(new EntityConverter());
            Register(new StringToJsonConverter());
            Register(new BytesToJsonConverter());
            Register(new StringToEnumConverter());
            Register(new LongToEnumConverter());
            Register(new ListConverter());
        }
        #endregion

        #region Methods
        public static void Register(IConverter converter)
        {
            _converters.Add(converter);
        }

        public static IConverter? Find(Type sourceType, Type targetType)
        {
            return _converters.FirstOrDefault(converter => converter.CanConvert(sourceType, targetType));
        }
        #endregion
    }
}
