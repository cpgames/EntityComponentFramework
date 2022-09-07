using cpGames.core.EntityComponentFramework.impl;
using cpGames.core.RapidIoC;

namespace cpGames.core.EntityComponentFramework
{
    public interface IComponent
    {
        #region Properties
        Entity? Entity { get; }

        ISignalOutcome ConnectedSignal { get; }
        ISignalOutcome DisconnectedSignal { get; }
        #endregion

        #region Methods
        Outcome Connect(Entity entity);
        Outcome Disconnect();
        #endregion
    }
}