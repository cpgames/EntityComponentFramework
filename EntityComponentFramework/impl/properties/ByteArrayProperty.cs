using System;
using System.Text;
using cpGames.core.RapidIoC;

namespace cpGames.core.EntityComponentFramework.impl
{
    public class ByteArrayProperty : Property<byte[]>
    {
        #region Properties
        public override object Data => _value;
        #endregion

        #region Methods
        protected override Outcome Convert(object data, out byte[] value)
        {
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
            return Outcome.Fail($"Failed to convert <{data.GetType().Name}> to <{nameof(String)}>.");
        }
        #endregion
    }
}