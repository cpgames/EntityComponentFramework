using ECS;

namespace cpGames.core.EntityComponentFramework
{
    public interface IWorld : IComponent
    {
        #region Properties
        Entity AuthenticationManagerEntity { get; }
        Entity AccountManagerEntity { get; }
        Entity ActorManagerEntity { get; }
        Entity AdmiralManagerEntity { get; }
        Entity AdmiralTemplateManagerEntity { get; }
        Entity BehaviorManagerEntity { get; }
        Entity BodyManagerEntity { get; }
        Entity CelestialManagerEntity { get; }
        Entity CorpManagerEntity { get; }
        Entity EncounterManagerEntity { get; }
        Entity FleetManagerEntity { get; }
        Entity GalaxyManagerEntity { get; }
        Entity InventoryManagerEntity { get; }
        Entity ItemManagerEntity { get; }
        Entity ItemTemplateManagerEntity { get; }
        Entity SectorManagerEntity { get; }
        Entity StationManagerEntity { get; }
        Entity WalletManagerEntity { get; }

        IAuthenticationManager AuthenticationManager { get; }
        IAccountManager AccountManager { get; }
        IActorManager ActorManager { get; }
        IAdmiralManager AdmiralManager { get; }
        IAdmiralTemplateManager AdmiralTemplateManager { get; }
        IBehaviorManager BehaviorManager { get; }
        IBodyManager BodyManager { get; }
        ICelestialManager CelestialManager { get; }
        ICorpManager CorpManager { get; }
        IEncounterManager EncounterManager { get; }
        IFleetManager FleetManager { get; }
        IGalaxyManager GalaxyManager { get; }
        IInventoryManager InventoryManager { get; }
        IItemManager ItemManager { get; }
        IItemTemplateManager ItemTemplateManager { get; }
        ISectorManager SectorManager { get; }
        IStationManager StationManager { get; }
        IWalletManager WalletManager { get; }
        #endregion
    }
}