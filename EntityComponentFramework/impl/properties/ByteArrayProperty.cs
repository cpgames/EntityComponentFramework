using System;
using System.Text;

namespace cpGames.core.EntityComponentFramework.impl
{
    public class ByteArrayProperty : Property<byte[]>
    {
        #region Constructors
        public ByteArrayProperty(Entity owner, string name, byte[] defaultValue) : base(owner, name, defaultValue) { }
        public ByteArrayProperty(Entity owner, string name) : base(owner, name, new byte[] { }) { }
        #endregion

        #region Methods
        protected override Outcome ConvertToValue(object? data, out byte[]? value)
        {
            if (data == null)
            {
                value = null;
                return Outcome.Success();
            }
            if (data is string str)
            {
                value = Encoding.Default.GetBytes(str);
                return Outcome.Success();
            }
            if (data is byte[] bytes)
            {
                value = bytes;
                return Outcome.Success();
            }
            value = default;
            return Outcome.Fail($"Failed to convert <{data.GetType().Name}> to <{nameof(String)}>.", this);
        }
        #endregion
    }
}