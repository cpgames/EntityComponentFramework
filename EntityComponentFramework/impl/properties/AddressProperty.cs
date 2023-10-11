namespace cpGames.core.EntityComponentFramework.impl
{
    public class AddressProperty : Property<Address>, IAddressProperty
    {
        #region Constructors
        public AddressProperty(Entity owner, string name, Address defaultValue) : base(owner, name, defaultValue) { }
        public AddressProperty(Entity owner, string name) : base(owner, name, Address.INVALID) { }
        #endregion

        #region Methods
        protected override Outcome ConvertToValue(object? data, out Address value)
        {
            if (data is Address address)
            {
                value = address;
                return Outcome.Success();
            }
            if (data is string str)
            {
                value = new Address(str);
                return Outcome.Success();
            }
            value = Address.INVALID;
            return Outcome.Success();
        }

        protected override Outcome ConvertToData(Address value, out object? data)
        {
            data = value.ToString();
            return Outcome.Success();
        }
        #endregion
    }
}