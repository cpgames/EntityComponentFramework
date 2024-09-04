namespace cpGames.core.EntityComponentFramework
{
    public interface IReferenceResolver : IComponent
    {
        #region Methods
        Outcome ResolveReference(Address address, out IComponent? component);
        Outcome ConvertReferenceToAddress(IComponent component, out Address address);
        #endregion
    }

    public interface IReferenceProperty : IProperty
    {
        #region Methods
        Outcome HasTarget();
        #endregion
    }

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