namespace cpGames.core.EntityComponentFramework
{
    public interface IEnumProperty<TEnum> : IProperty<TEnum>
        where TEnum : struct { }
}