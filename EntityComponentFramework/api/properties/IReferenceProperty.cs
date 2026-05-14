namespace cpGames.core.EntityComponentFramework
{
    public interface IReferenceResolver : IComponent
    {
        #region Methods
        Outcome ResolveReference<TComponent>(Id id, out TComponent? component)
            where TComponent : class, IComponent;

        Outcome ConvertReferenceToId(IComponent component, out Id id);
        #endregion
    }

    public interface IReferenceProperty : IProperty { }

    public interface IReferenceProperty<TComponent> : IProperty<TComponent?>, IReferenceProperty
        where TComponent : class, IComponent
    {
        #region Methods
        Outcome GetOtherComponent<TOtherComponent>(out TOtherComponent? otherComponent)
            where TOtherComponent : class, IComponent;

        Outcome GetDerivedNonDefault<TDerivedComponent>(out TDerivedComponent? derivedComponent)
            where TDerivedComponent : class, TComponent;
        #endregion
    }
}