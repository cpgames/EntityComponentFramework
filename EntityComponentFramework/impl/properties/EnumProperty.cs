using System;
using cpGames.core.RapidIoC;

namespace cpGames.core.EntityComponentFramework.impl
{
    public abstract class EnumProperty<TEnum> : Property<TEnum>, IEnumProperty<TEnum>
        where TEnum : struct
    {
        #region IEnumProperty<TEnum> Members
        public override object Data => _value.ToString();
        #endregion

        #region Methods
        protected override Outcome Convert(object data, out TEnum value)
        {
            if (data is string)
            {
                return Enum.TryParse((string)data, out value) ?
                    Outcome.Success() :
                    Outcome.Fail($"Failed to convert <{(string)data}> to <{typeof(TEnum).Name}>.");
            }
            if (data is long dataLong)
            {
                value = (TEnum)Enum.GetValues(typeof(TEnum)).GetValue(dataLong);
                return Outcome.Success();
            }
            value = default;
            return Outcome.Fail($"Failed to convert <{data}> to <{typeof(TEnum).Name}>.");
        }
        #endregion
    }
}