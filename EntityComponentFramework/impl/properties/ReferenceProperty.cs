using cpGames.core.RapidIoC;
using ECS;

namespace cpGames.core.EntityComponentFramework.impl
{
    public class ReferenceProperty : Property<Address>, IReferenceProperty
    {
        #region Fields
        private IDependencyContainer _dependencyContainer;
        private Entity _targetEntity;
        #endregion

        #region IReferenceProperty Members
        public Entity TargetEntity => _targetEntity;
        public Address TargetAddress => HasTarget() ? _targetEntity.Address : Address.INVALID;
        public Id TargetId => HasTarget() ? _targetEntity.Id : Id.INVALID;

        public override object Data => _value.Bytes;

        public Outcome SetId<TEntityManager>(Id id) where TEntityManager : IEntityManager
        {
            return
                Rapid.GetBindingValue<TEntityManager>(Rapid.RootKey, out var manager) &&
                Set(manager.CreateAddress(id));
        }

        public Outcome HasTarget()
        {
            return TargetEntity != null ?
                Outcome.Success() :
                Outcome.Fail($"Reference {Name} is not established.");
        }

        public Outcome GetTargetEntity(out Entity target)
        {
            var hasTargetOutcome = HasTarget();
            if (!hasTargetOutcome)
            {
                target = null;
                return hasTargetOutcome;
            }
            target = TargetEntity;
            return Outcome.Success();
        }

        public Outcome GetTargetComponent<TComponent>(out TComponent targetComponent) where TComponent : class, IComponent
        {
            targetComponent = null;
            return
                GetTargetEntity(out var targetEntity) &&
                targetEntity.GetComponent(out targetComponent);
        }
        #endregion

        #region Methods
        protected override Outcome UpdateValue(Address value)
        {
            if (_dependencyContainer != null)
            {
                var removeOutcome = _dependencyContainer.Remove(Owner.Address);
                if (!removeOutcome)
                {
                    return removeOutcome;
                }
            }
            _dependencyContainer = null;
            _targetEntity = null;
            if (value.IsValid)
            {
                var addOutcome =
                    Owner.Root.GetChildByAddress(value, out _targetEntity) &&
                    _targetEntity.GetComponent(out _dependencyContainer) &&
                    _dependencyContainer.Add(Owner);
                if (!addOutcome)
                {
                    return addOutcome;
                }
            }
            return base.UpdateValue(value);
        }

        protected override Outcome Convert(object data, out Address value)
        {
            if (data is byte[] bytes)
            {
                value = new Address(bytes);
                return Outcome.Success();
            }
            return base.Convert(data, out value);
        }
        #endregion
    }
}