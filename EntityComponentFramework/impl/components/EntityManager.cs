using cpGames.core.RapidIoC;
using ECS;

namespace cpGames.core.EntityComponentFramework.impl
{
    public abstract class EntityManager : Component, IEntityManager
    {
        #region IEntityManager Members
        [Inject(GameIds.WORLD)] public Entity WorldEntity { get; set; }
        [Inject] public IWorld World { get; set; }

        public Address CreateAddress(Id id)
        {
            return Entity.Address.Append(id);
        }
        #endregion

        #region Methods
        protected override Outcome ConnectInternal()
        {
            return
                Entity.ChildAddedSignal.AddCommand(AddRequiredComponents, this) &&
                base.ConnectInternal();
        }

        protected override Outcome DisconnectInternal()
        {
            return
                Entity.ChildAddedSignal.RemoveCommand(this) &&
                base.DisconnectInternal();
        }

        protected abstract Outcome AddRequiredComponents(Entity target);
        #endregion
    }
}