namespace cpGames.core.EntityComponentFramework.impl
{
    public class FloatProperty : Property<float>, IFloatProperty
    {
        #region Constructors
        public FloatProperty(Entity owner, string name) : base(owner, name, 0f) { }
        #endregion

        #region IFloatProperty Members
        public Outcome Add(float value)
        {
            return
                Get(out var currentValue) &&
                Set(currentValue + value);
        }

        public Outcome Subtract(float value)
        {
            return
                Get(out var currentValue) &&
                Set(currentValue - value);
        }
        #endregion
    }
}