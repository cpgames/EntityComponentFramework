﻿using System;
using cpGames.core.EntityComponentFramework.impl;
using cpGames.core.RapidIoC;

namespace cpGames.core.EntityComponentFramework
{
    public interface IProperty
    {
        #region Properties
        string Name { get; }
        Entity Owner { get; }
        ISignalOutcome ValueGetSignal { get; }
        ISignalOutcome<object?> BeginValueSetSignal { get; }
        ISignalOutcome<object?> EndValueSetSignal { get; }
        Type ValueType { get; }
        #endregion

        #region Methods
        Outcome SetData(object? data);
        Outcome GetData(out object? data);
        Outcome GetValueObj(out object? valueObj);
        Outcome UpdateValue();
        Outcome ValueEquals(object? value);
        string ValueToString();
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
        Outcome Get(out TValue? value);
        Outcome ConnectToProperty(IProperty<TValue> otherProperty);
        Outcome DisconnectProperties();
        Outcome Equals(TValue value);
        #endregion
    }
}