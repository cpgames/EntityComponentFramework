namespace cpGames.core.EntityComponentFramework.impl
{
    public abstract class EnumProperty<TEnum> : Property<TEnum>, IEnumProperty<TEnum>
        where TEnum : struct
    {
        #region Constructors
        protected EnumProperty(Entity owner, string name, TEnum defaultValue) : base(owner, name, defaultValue)
        {
            _converters.Add(new StringToEnumConverter<TEnum>());
            _converters.Add(new LongToEnumConverter<TEnum>());
        }
        #endregion

        #region Methods
        protected override Outcome ConvertToData(TEnum value, out object? data)
        {
            data = value.ToString();
            return Outcome.Success();
        }
        #endregion
    }
}