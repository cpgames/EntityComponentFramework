namespace cpGames.core.EntityComponentFramework.impl
{
    public class IntProperty : Property<int>, IIntProperty
    {
        #region Constructors
        public IntProperty(Entity owner, string name) : base(owner, name, 0) { }
        #endregion

        #region IIntProperty Members
        public Outcome Add(int value)
        {
            return
                Get(out var currentValue) &&
                Set(currentValue + value);
        }

        public Outcome Subtract(int value)
        {
            return
                Get(out var currentValue) &&
                Set(currentValue - value);
        }
        #endregion
    }
}