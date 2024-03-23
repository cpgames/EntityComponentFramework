using System;

namespace cpGames.core.EntityComponentFramework.impl
{
    public abstract class EnumProperty<TEnum> : Property<TEnum>, IEnumProperty<TEnum>
        where TEnum : struct
    {
        #region Constructors
        protected EnumProperty(Entity owner, string name) : base(owner, name, default) { }
        protected EnumProperty(Entity owner, string name, TEnum defaultValue) : base(owner, name, defaultValue) { }
        #endregion

        #region Methods
        protected override Outcome ConvertToValue(object? data, out TEnum value)
        {
            if (data is TEnum)
            {
                value = (TEnum)data;
                return Outcome.Success();
            }
            if (data is string)
            {
                return Enum.TryParse((string)data, out value) ?
                    Outcome.Success() :
                    Outcome.Fail($"Failed to convert <{(string)data}> to <{typeof(TEnum).Name}>.", this);
            }
            if (data is long dataLong)
            {
                value = (TEnum)Enum.GetValues(typeof(TEnum)).GetValue(dataLong);
                return Outcome.Success();
            }
            return base.ConvertToValue(data, out value);
        }

        protected override Outcome ConvertToData(TEnum value, out object? data)
        {
            data = value.ToString();
            return Outcome.Success();
        }
        #endregion
    }
}