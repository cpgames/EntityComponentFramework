using System;

namespace cpGames.core.EntityComponentFramework
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PropertyAttribute : Attribute
    {
        #region Properties
        public string Name { get; }
        public Type? Type { get; }
        public object? DefaultValue { get; }
        #endregion

        #region Constructors
        public PropertyAttribute(string name, Type? type = default, object? defaultValue = default)
        {
            Name = name;
            Type = type;
            DefaultValue = defaultValue;
        }
        #endregion
    }
}