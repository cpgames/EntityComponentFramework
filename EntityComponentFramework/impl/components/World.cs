using System.Collections.Generic;
using System.Net;
using cpGames.core;
using cpGames.core.EntityComponentFramework;
using cpGames.core.EntityComponentFramework.impl;
using cpGames.core.RapidIoC;
using ECS;
using Newtonsoft.Json;

namespace cpGames.core.EntityComponentFramework.impl
{
    public class World : Component, IWorld
    {
        #region Fields
        public const long TICK_INTERVAL_MS = 500;

        private static IKey _key;

        private Entity _authenticationManagerEntity;
        private Entity _accountManagerEntity;
        private Entity _actorManagerEntity;
        private Entity _admiralManagerEntity;
        private Entity _admiralTemplateManagerEntity;
        private Entity _behaviorManagerEntity;
        private Entity _bodyManagerEntity;
        private Entity _celestialManagerEntity;
        private Entity _corpManagerEntity;
        private Entity _encounterManagerEntity;
        private Entity _fleetManagerEntity;
        private Entity _galaxyManagerEntity;
        private Entity _inventoryManagerEntity;
        private Entity _itemManagerEntity;
        private Entity _itemTemplateManagerEntity;
        private Entity _sectorManagerEntity;
        private Entity _stationManagerEntity;
        private Entity _walletManagerEntity;

        private AuthenticationManager _authenticationManager;
        private AccountManager _accountManager;
        private ActorManager _actorManager;
        private AdmiralManager _admiralManager;
        private AdmiralTemplateManager _admiralTemplateManager;
        private BehaviorManager _behaviorManager;
        private BodyManager _bodyManager;
        private CelestialManager _celestialManager;
        private CorpManager _corpManager;
        private EncounterManager _encounterManager;
        private FleetManager _fleetManager;
        private GalaxyManager _galaxyManager;
        private InventoryManager _inventoryManager;
        private ItemManager _itemManager;
        private ItemTemplateManager _itemTemplateManager;
        private SectorManager _sectorManager;
        private StationManager _stationManager;
        private WalletManager _walletManager;
        #endregion

        #region Properties
        public static IKey Key => _key;
        #endregion

        #region Constructors
        public World()
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Converters = new List<JsonConverter>
                {
                    new StringPropertyConverter(),
                    new ItemTypePropertyConverter(),
                    new JsonPropertyConverter<ItemSettingsCollection>(),
                    new JsonPropertyConverter<BehaviorData>()
                }
            };
        }
        #endregion

        #region IWorld Members
        public Entity AuthenticationManagerEntity => _authenticationManagerEntity;
        public Entity AccountManagerEntity => _accountManagerEntity;
        public Entity ActorManagerEntity => _actorManagerEntity;
        public Entity AdmiralManagerEntity => _admiralManagerEntity;
        public Entity AdmiralTemplateManagerEntity => _admiralTemplateManagerEntity;
        public Entity BehaviorManagerEntity => _behaviorManagerEntity;
        public Entity BodyManagerEntity => _bodyManagerEntity;
        public Entity CelestialManagerEntity => _celestialManagerEntity;
        public Entity CorpManagerEntity => _corpManagerEntity;
        public Entity EncounterManagerEntity => _encounterManagerEntity;
        public Entity FleetManagerEntity => _fleetManagerEntity;
        public Entity GalaxyManagerEntity => _galaxyManagerEntity;
        public Entity InventoryManagerEntity => _inventoryManagerEntity;
        public Entity ItemManagerEntity => _itemManagerEntity;
        public Entity ItemTemplateManagerEntity => _itemTemplateManagerEntity;
        public Entity SectorManagerEntity => _sectorManagerEntity;
        public Entity StationManagerEntity => _stationManagerEntity;
        public Entity WalletManagerEntity => _walletManagerEntity;
        public IAuthenticationManager AuthenticationManager => _authenticationManager;
        public IAccountManager AccountManager => _accountManager;
        public IActorManager ActorManager => _actorManager;
        public IAdmiralManager AdmiralManager => _admiralManager;
        public IAdmiralTemplateManager AdmiralTemplateManager => _admiralTemplateManager;
        public IBehaviorManager BehaviorManager => _behaviorManager;
        public IBodyManager BodyManager => _bodyManager;
        public ICelestialManager CelestialManager => _celestialManager;
        public ICorpManager CorpManager => _corpManager;
        public IEncounterManager EncounterManager => _encounterManager;
        public IFleetManager FleetManager => _fleetManager;
        public IGalaxyManager GalaxyManager => _galaxyManager;
        public IInventoryManager InventoryManager => _inventoryManager;
        public IItemManager ItemManager => _itemManager;
        public IItemTemplateManager ItemTemplateManager => _itemTemplateManager;
        public ISectorManager SectorManager => _sectorManager;
        public IStationManager StationManager => _stationManager;
        public IWalletManager WalletManager => _walletManager;
        #endregion

        #region Methods
        protected override Outcome ConnectInternal()
        {
            return
                Rapid.KeyFactoryCollection.Create(GameIds.WORLD, out _key) &&
                Rapid.Bind(Key, Key, Entity) &&
                AddManager(
                    GameIds.AUTHENTICATION,
                    "Authentications",
                    out _authenticationManagerEntity,
                    out _authenticationManager) &&
                AddManager(
                    GameIds.ACCOUNT,
                    "Accounts",
                    out _accountManagerEntity,
                    out _accountManager) &&
                AddManager(
                    GameIds.ACTOR,
                    "Actors",
                    out _actorManagerEntity,
                    out _actorManager) &&
                AddManager(
                    GameIds.ADMIRAL,
                    "Admirals",
                    out _admiralManagerEntity,
                    out _admiralManager) &&
                AddManager(
                    GameIds.ADMIRAL_TEMPLATE,
                    "AdmiralTemplates",
                    out _admiralTemplateManagerEntity,
                    out _admiralTemplateManager) &&
                AddManager(
                    GameIds.BEHAVIOR,
                    "Behaviors",
                    out _behaviorManagerEntity,
                    out _behaviorManager) &&
                AddManager(
                    GameIds.BODY,
                    "Bodies",
                    out _bodyManagerEntity,
                    out _bodyManager) &&
                AddManager(
                    GameIds.CELESTIAL,
                    "Celestials",
                    out _celestialManagerEntity,
                    out _celestialManager) &&
                AddManager(
                    GameIds.CORP,
                    "Corps",
                    out _corpManagerEntity,
                    out _corpManager) &&
                AddManager(
                    GameIds.ENCOUNTER,
                    "Encounters",
                    out _encounterManagerEntity,
                    out _encounterManager) &&
                AddManager(
                    GameIds.FLEET,
                    "Fleets",
                    out _fleetManagerEntity,
                    out _fleetManager) &&
                AddManager(
                    GameIds.GALAXY,
                    "Galaxies",
                    out _galaxyManagerEntity,
                    out _galaxyManager) &&
                AddManager(
                    GameIds.INVENTORY,
                    "Inventories",
                    out _inventoryManagerEntity,
                    out _inventoryManager) &&
                AddManager(
                    GameIds.ITEM,
                    "Items",
                    out _itemManagerEntity,
                    out _itemManager) &&
                AddManager(
                    GameIds.ITEM_TEMPLATE,
                    "ItemTemplates",
                    out _itemTemplateManagerEntity,
                    out _itemTemplateManager) &&
                AddManager(
                    GameIds.SECTOR,
                    "Sectors",
                    out _sectorManagerEntity,
                    out _sectorManager) &&
                AddManager(
                    GameIds.STATION,
                    "Stations",
                    out _stationManagerEntity,
                    out _stationManager) &&
                AddManager(
                    GameIds.WALLET,
                    "Wallets",
                    out _walletManagerEntity,
                    out _walletManager) &&
                base.ConnectInternal();
        }

        protected override Outcome DisconnectInternal()
        {
            return
                Rapid.Unbind(Key, Key) &&
                base.DisconnectInternal();
        }

        private Outcome AddManager<TManager>(
            Id id,
            string name,
            out Entity managerEntity,
            out TManager manager)
            where TManager : class, IEntityManager
        {
            manager = default;
            return
                Entity.AddChild(id, name, out managerEntity) &&
                managerEntity.AddComponent(out manager);
        }
        #endregion
    }
}