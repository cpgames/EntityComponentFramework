namespace cpGames.core.EntityComponentFramework.impl
{
    public class FloatProperty : Property<float>
    {
        #region Constructors
        public FloatProperty(Entity owner, string name, float defaultValue = 0f) : base(owner, name, defaultValue) { }
        #endregion
    }
}