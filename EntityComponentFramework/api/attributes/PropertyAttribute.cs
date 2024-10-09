using System;

namespace cpGames.core.EntityComponentFramework
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PropertyAttribute : Attribute
    {
        #region Properties
        public string Name { get; }
        public Type? Type { get; }

        public int Order { get; }
        #endregion

        #region Constructors
        public PropertyAttribute(string name, Type? type = default, int order = 0)
        {
            Name = name;
            Type = type;
            Order = order;
        }
        #endregion
    }
}