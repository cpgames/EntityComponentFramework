using System;
using System.Globalization;
using cpGames.core.RapidIoC;

namespace cpGames.core.EntityComponentFramework.impl
{
    public class DateTimeProperty : Property<DateTime>, IDateTimeProperty
    {
        #region IDateTimeProperty Members
        public override object Data => _value.ToString("yyyy-MM-dd HH:mm:ss");

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
        protected override Outcome Convert(object data, out DateTime value)
        {
            if (data is DateTime dateTime)
            {
                value = dateTime;
            }
            else if (!DateTime.TryParse((string)data, CultureInfo.InvariantCulture, DateTimeStyles.None, out value))
            {
                return Outcome.Fail($"Failed to convert {(string)data} to DateTime.");
            }
            return Outcome.Success();
        }
        #endregion
    }
}