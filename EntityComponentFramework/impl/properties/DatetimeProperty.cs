using System;
using System.Globalization;

namespace cpGames.core.EntityComponentFramework.impl
{
    public class DateTimeProperty : Property<DateTime>, IDateTimeProperty
    {
        #region Constructors
        public DateTimeProperty(Entity owner, string name) : base(owner, name, default) 
        {
            _converters.Add(new StringToDateTimeConverter());
        }
        #endregion

        #region IDateTimeProperty Members
        public string ValueToHHmm()
        {
            return _value.ToString("HH:mm");
        }

        public override string ValueToString()
        {
            return _value.ToString("yyyy-MM-dd HH:mm:ss");
        }
        #endregion

        #region Methods
        protected override Outcome ConvertToData(DateTime value, out object? data)
        {
            data = value.ToString("yyyy-MM-dd HH:mm:ss");
            return Outcome.Success();
        }
        #endregion
    }
}