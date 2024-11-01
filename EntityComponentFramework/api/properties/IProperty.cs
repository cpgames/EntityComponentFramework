using System;
using cpGames.core.EntityComponentFramework.impl;
using cpGames.core.RapidIoC;

namespace cpGames.core.EntityComponentFramework
{
    public interface IProperty
    {
        #region Properties
        string Name { get; }
        Entity Owner { get; }
        int Index { get; set; }
        ISignalOutcome ValueGetSignal { get; }
        ISignalOutcome<object?> BeginValueSetSignal { get; }
        ISignalOutcome<object?> EndValueSetSignal { get; }
        Type ValueType { get; }
        #endregion

        #region Methods
        Outcome Connect();
        Outcome Disconnect();
        Outcome SetData(object? data);
        Outcome GetData(out object? data);
        Outcome GetValueObj(out object? valueObj);
        Outcome UpdateValue();
        Outcome ValueEquals(object? value, out bool result);
        Outcome ValueEquals(IProperty otherProperty, out bool result);
        Outcome ValueNotEquals(object? value);
        Outcome ValueNotEquals(IProperty otherProperty);
        Outcome IsDefault(out bool result);
        Outcome ResetToDefault();
        string ValueToString();
        Outcome Link(IProperty otherProperty);
        Outcome Unlink(bool reset = true);
        bool IsLinked();
        #endregion
    }

    public interface IProperty<TValue> : IProperty
    {
        #region Properties
        TValue Value { get; set; }
        #endregion

        #region Methods
        Outcome Set(TValue value);
        Outcome Set(IProperty<TValue> otherProperty);
        Outcome SetDefaultValue(TValue value);
        Outcome Get(out TValue? value);
        Outcome GetNonDefault(out TValue? value);
        Outcome Equals(TValue value, out bool result);
        #endregion
    }
}