using System.Collections.Generic;
using cpGames.core.EntityComponentFramework.impl;
using cpGames.core.RapidIoC;

namespace cpGames.core.EntityComponentFramework
{
    public interface IComponent
    {
        #region Properties
        bool IsConnected { get; }
        Entity Entity { get; }
        Id Id { get; }
        IEnumerable<IProperty> Properties { get; }

        ISignalOutcome<IComponent> BeginConnectedSignal { get; }
        ISignalOutcome<IComponent> EndConnectedSignal { get; }
        ISignalOutcome<IComponent> BeginDisconnectedSignal { get; }
        ISignalOutcome<IComponent> EndDisconnectedSignal { get; }
        #endregion

        #region Methods
        Outcome Connect(Entity entity);
        Outcome Disconnect();
        Outcome GetEntity(out Entity entity);
        #endregion
    }
}