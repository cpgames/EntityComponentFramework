namespace cpGames.core.EntityComponentFramework.impl
{
    public class ByteArrayProperty : Property<byte[]>
    {
        #region Constructors
        public ByteArrayProperty(Entity owner, string name) : base(owner, name, new byte[] { })
        {
            _converters.Add(new StringToByteArrayConverter());
        }
        #endregion
    }
}