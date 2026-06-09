namespace cpGames.core.EntityComponentFramework
{
    public interface IReferenceResolver : IComponent
    {
        #region Methods
        Outcome ResolveReference<TComponent>(Id id, out TComponent? component)
            where TComponent : class, IComponent;

        Outcome ResolveEntity(Id id, out impl.Entity? entity);

        Outcome ConvertReferenceToId(IComponent component, out Id id);
        #endregion
    }
}
