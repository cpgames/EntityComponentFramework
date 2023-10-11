using System;

namespace cpGames.core.EntityComponentFramework.impl
{
    public class BoolProperty : Property<bool>, IBoolProperty
    {
        #region Constructors
        public BoolProperty(Entity owner, string name, bool defaultValue = false) : base(owner, name, defaultValue) { }
        #endregion

        #region Methods
        protected override Outcome ConvertToValue(object? data, out bool value)
        {
            value = data != null && ((IConvertible)data).ToByte(null) == 1;
            return Outcome.Success();
        }

        protected override Outcome ConvertToData(bool value, out object? data)
        {
            data = value ? 1 : 0;
            return Outcome.Success();
        }
        #endregion
    }
}