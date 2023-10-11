namespace cpGames.core.EntityComponentFramework.impl
{
    public class IntProperty : Property<int>
    {
        public IntProperty(Entity owner, string name, int defaultValue = 0) : base(owner, name, defaultValue) { }
    }
}