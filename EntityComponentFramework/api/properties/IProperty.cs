using cpGames.core.EntityComponentFramework.impl;
using cpGames.core.RapidIoC;

namespace cpGames.core.EntityComponentFramework
{
    public interface IProperty : IComponent
    {
        #region Properties
        string Name { get; }
        object Data { get; }
        Entity Owner { get; }
        ISignalOutcome BeginModifySignal { get; }
        ISignalOutcome EndModifySignal { get; }
        #endregion

        #region Methods
        Outcome SetObject(object data);
        Outcome UpdateValue();
        bool ValueEquals(object value);
        string ValueToString();
        #endregion
    }

    public interface IProperty<TValue> : IProperty
    {
        #region Properties
        TValue Value { get; }
        #endregion

        #region Methods
        Outcome Set(TValue value);
        TValue Get();
        Outcome ConnectToProperty(IProperty<TValue> otherProperty);
        Outcome DisconnectProperties();
        #endregion
    }
}