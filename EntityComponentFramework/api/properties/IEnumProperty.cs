namespace cpGames.core.EntityComponentFramework
{
    public interface IEnumProperty : IProperty { }
    public interface IEnumProperty<TEnum> : IEnumProperty, IProperty<TEnum>
        where TEnum : struct { }
}