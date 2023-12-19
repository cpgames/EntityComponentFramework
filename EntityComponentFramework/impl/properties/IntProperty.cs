namespace cpGames.core.EntityComponentFramework.impl
{
    public class IntProperty : Property<int>
    {
        #region Constructors
        public IntProperty(Entity owner, string name) : this(owner, name, 0) { }
        public IntProperty(Entity owner, string name, int defaultValue) : base(owner, name, defaultValue) { }
        #endregion
    }
}