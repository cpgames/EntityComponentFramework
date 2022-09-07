using System;

namespace cpGames.core.EntityComponentFramework
{
    [AttributeUsage(AttributeTargets.Property)]
    public class RequiredComponentAttribute : Attribute
    {
        #region Fields
        public bool searchParent = false;
        public bool addIfMissing = false;
        #endregion
    }
}