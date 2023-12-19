namespace cpGames.core.EntityComponentFramework.impl
{
    public class LongProperty : Property<long>
    {
        #region Constructors
        public LongProperty(Entity owner, string name) : this(owner, name, 0) { }
        public LongProperty(Entity owner, string name, long defaultValue) : base(owner, name, defaultValue) { }
        #endregion
    }
}