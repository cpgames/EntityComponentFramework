using System;
using cpGames.core.EntityComponentFramework.impl;
using cpGames.core.RapidIoC;

namespace cpGames.core.EntityComponentFramework
{
    public interface IReadonlyProperty
    {
        #region Properties
        string Name { get; }
        Entity Owner { get; }
        Type ValueType { get; }
        ISignalOutcome ValueGetSignal { get; }
        ISignalOutcome<object?> BeginValueSetSignal { get; }
        ISignalOutcome<object?> EndValueSetSignal { get; }
        #endregion

        #region Methods
        Outcome GetData(out object? data);
        Outcome GetValueObj(out object? valueObj);
        Outcome ValueEquals(object? value, out bool result);
        Outcome ValueEquals(IProperty otherProperty, out bool result);
        Outcome ValueNotEquals(object? value);
        Outcome ValueNotEquals(IProperty otherProperty);
        Outcome IsDefault(out bool result);
        string ValueToString();
        #endregion
    }

    public interface IProperty : IReadonlyProperty
    {
        #region Properties
        int Index { get; set; }
        #endregion

        #region Methods
        Outcome Connect();
        Outcome Disconnect();
        Outcome SetData(object? data);
        Outcome UpdateValue();
        Outcome ResetToDefault();
        Outcome Link(IProperty otherProperty);
        Outcome Unlink(bool reset = true);
        bool IsLinked();
        #endregion
    }

    public interface IReadonlyProperty<TValue> : IReadonlyProperty
    {
        #region Methods
        Outcome Get(out TValue? value);
        Outcome GetNonDefault(out TValue? value);
        Outcome Equals(TValue value, out bool result);
        #endregion
    }

    public interface IProperty<TValue> : IReadonlyProperty<TValue>, IProperty
    {
        #region Properties
        TValue Value { get; set; }
        #endregion

        #region Methods
        Outcome Set(TValue value);
        Outcome Set(IProperty<TValue> otherProperty);
        Outcome SetDefaultValue(TValue value);
        Outcome AddConverter(IConverter converter);
        #endregion
    }
}