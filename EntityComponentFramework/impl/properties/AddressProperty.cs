namespace cpGames.core.EntityComponentFramework.impl
{
    public class AddressProperty : Property<Address>, IAddressProperty
    {
        #region Constructors
        public AddressProperty(Entity owner, string name) : base(owner, name, Address.INVALID)
        {
        }
        #endregion

        #region Methods
        protected override Outcome ConvertToData(Address value, out object? data)
        {
            data = value.ToString();
            return Outcome.Success();
        }
        #endregion
    }
}