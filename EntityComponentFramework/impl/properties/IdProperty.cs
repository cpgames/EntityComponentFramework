namespace cpGames.core.EntityComponentFramework.impl
{
    public class IdProperty : Property<Id>, IIdProperty
    {
        #region Constructors
        public IdProperty(Entity owner, string name, Id defaultValue) : base(owner, name, defaultValue) { }
        public IdProperty(Entity owner, string name) : base(owner, name, Id.INVALID) { }
        #endregion

        #region Methods
        protected override Outcome ConvertToValue(object? data, out Id value)
        {
            if (data is Id id)
            {
                value = id;
                return Outcome.Success();
            }
            if (data is string str)
            {
                return Id.TryParse(str, out value).Append(this);
            }
            value = Id.INVALID;
            return Outcome.Success();
        }

        protected override Outcome ConvertToData(Id value, out object? data)
        {
            data = value.ToString();
            return Outcome.Success();
        }
        #endregion
    }
}