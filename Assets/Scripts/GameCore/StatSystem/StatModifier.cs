using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

[Serializable]
public class StatModifier
{
    public float Value;
    public StatModType Type;
    public StatParam Param;
    public readonly int Order;
    public object Source;

    public StatModifier(float value, StatModType type, StatParam param, int order, object source)
    {
        Value = value;
        Type = type;
        Param = param;
        Order = order;
        Source = source;
    }

    public StatModifier(float value, StatModType type, StatParam param) : this(value, type, param, (int)type, null) { }

    public StatModifier(float value, StatModType type, StatParam param, int order) : this(value, type, param, order, null) { }

    public StatModifier(float value, StatModType type, StatParam param, object source) : this(value, type, param, (int)type, source) { }

    private string StringifyAmount()
    {
        string modAmount = "";
        switch (Type)
        {
            case StatModType.Flat:
                modAmount = $"+{Value}";
                break;
            case StatModType.PercentAdd:
                modAmount = $"+{Value}%";
                break;
            case StatModType.PercentMult:
                modAmount = $"X{Value}%";
                break;
            default:
                modAmount = $"Unknown type {Type}";
                break;
        }
        return modAmount;
    }
    private string StringifyParameter()
    {
        return Param.ToString();
    }
    public override string ToString()
    {
        return $"{StringifyAmount()} {StringifyParameter()}";
    }
    public string ToStringWithSource()
    {
        return $"{StringifyAmount()} {StringifyParameter()} : {Source}";
    }
    public string ToStringWithBreak()
    {
        return $"{StringifyAmount()}\n{StringifyParameter()}";
    }
    public Sprite GetSprite()
    {
        if ((int)Param < Globals.StatModIconsSt.Count)
            return Globals.StatModIconsSt[(int)Param];
        else return null;
    }
    public void CombineWith(StatModifier stat)
    {
        if (stat.Param != Param)
            return;
        if (stat.Type != Type)
            return;
        Value += stat.Value;
    }
}