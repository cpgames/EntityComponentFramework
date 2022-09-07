using System;

namespace cpGames.core.EntityComponentFramework
{
    public interface IDateTimeProperty : IProperty<DateTime>
    {
        #region Methods
        string ValueToHHmm();
        #endregion
    }
}