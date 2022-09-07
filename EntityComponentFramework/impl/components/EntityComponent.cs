using System.Collections.Generic;
using System.Linq;
using cpGames.core.RapidIoC;
using Newtonsoft.Json;

namespace cpGames.core.EntityComponentFramework.impl
{
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class EntityComponent : Component, IEntityComponent
    {
        #region Fields
        private StringProperty? _name;
        #endregion

        #region IEntityComponent Members
        [Inject(GameIds.WORLD)] public Entity? WorldEntity { get; set; }
        public IEnumerable<Entity> Properties =>
            Entity != null ?
                Entity.Children.Where(x => x.HasComponent<IProperty>()) :
                Enumerable.Empty<Entity>();
        public IEnumerable<Entity> Dependencies =>
            Entity != null ?
                Entity.Children.Where(x => x.HasComponent<IDependencyContainer>()) :
                Enumerable.Empty<Entity>();
        [Inject] public IWorld? World { get; set; }
        [JsonProperty(Order = -1)] public IStringProperty Name => _name;
        public Id EntityType => Entity?.Parent?.Id ?? Id.INVALID;
        public byte EntityTypeByte => EntityType.Length == 1 ? EntityType.Bytes[0] : GameIds.NONE;
        #endregion

        #region Methods
        protected virtual Outcome OnNameChange()
        {
            Entity.Name = Name.Get();
            return Outcome.Success();
        }

        protected override Outcome ConnectInternal()
        {
            if (Entity == null)
            {
                return Outcome.Fail("Entity is null");
            }
            var setNameOutcome =
                Entity.AddChildWithComponent("name", out _name) &&
                _name!.Set(Entity.Name);
            if (!setNameOutcome)
            {
                return setNameOutcome;
            }
            return
                Name.EndModifySignal.AddCommand(OnNameChange, this) &&
                base.ConnectInternal();
        }

        protected override Outcome DisconnectInternal()
        {
            return
                Name.EndModifySignal.RemoveCommand(this) &&
                base.DisconnectInternal();
        }

        public override string ToString()
        {
            return $"{Name}";
        }
        #endregion
    }
}