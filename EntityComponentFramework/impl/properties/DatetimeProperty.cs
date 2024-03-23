using System;
using System.Globalization;

namespace cpGames.core.EntityComponentFramework.impl
{
    public class DateTimeProperty : Property<DateTime>, IDateTimeProperty
    {
        #region Constructors
        public DateTimeProperty(Entity owner, string name) : this(owner, name, default) { }
        public DateTimeProperty(Entity owner, string name, DateTime defaultValue) : base(owner, name, defaultValue) { }
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
        protected override Outcome ConvertToValue(object? data, out DateTime value)
        {
            if (data == null)
            {
                value = default;
                return Outcome.Success();
            }
            if (data is DateTime dateTime)
            {
                value = dateTime;
            }
            else if (!DateTime.TryParse(
                         (string)data,
                         CultureInfo.InvariantCulture,
                         DateTimeStyles.None,
                         out value))
            {
                return Outcome.Fail($"Failed to convert {(string)data} to DateTime.", this);
            }
            return Outcome.Success();
        }

        protected override Outcome ConvertToData(DateTime value, out object? data)
        {
            data = value.ToString("yyyy-MM-dd HH:mm:ss");
            return Outcome.Success();
        }
        #endregion
    }
}