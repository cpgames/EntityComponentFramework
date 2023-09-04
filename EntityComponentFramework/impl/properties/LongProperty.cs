namespace cpGames.core.EntityComponentFramework.impl
{
    public class LongProperty : Property<long>
    {
        #region Constructors
        public LongProperty(Entity owner, string name, long defaultValue = 0) : base(owner, name, defaultValue) { }
        #endregion
    }
}