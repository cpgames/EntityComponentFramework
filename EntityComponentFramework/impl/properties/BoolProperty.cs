using System;
using cpGames.core.RapidIoC;

namespace cpGames.core.EntityComponentFramework.impl
{
    public class BoolProperty : Property<bool>, IBoolProperty
    {
        #region IBoolProperty Members
        public override object Data => _value ? 1 : 0;
        #endregion

        #region Methods
        protected override Outcome Convert(object data, out bool value)
        {
            value = data != null && ((IConvertible)data).ToByte(null) == 1;
            return Outcome.Success();
        }
        #endregion
    }
}