namespace cpGames.core.EntityComponentFramework.impl
{
    public class IdProperty : Property<Id>, IIdProperty
    {
        #region Constructors
        public IdProperty(Entity owner, string name) : base(owner, name, Id.INVALID) 
        {
            _converters.Add(new StringToIdConverter());
        }
        #endregion

        #region Methods
        protected override Outcome ConvertToData(Id value, out object? data)
        {
            data = value.ToString();
            return Outcome.Success();
        }
        #endregion
    }
}