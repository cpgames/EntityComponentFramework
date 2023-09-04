using System;

namespace cpGames.core.EntityComponentFramework
{
    [AttributeUsage(AttributeTargets.Property)]
    public class RequiredComponentAttribute : Attribute
    {
        #region Properties
        public Type? Type { get; }
        #endregion

        #region Constructors
        public RequiredComponentAttribute(Type? type = default)
        {
            Type = type;
        }
        #endregion
    }
}