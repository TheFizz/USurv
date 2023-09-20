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
    public string Source;

    public StatModifier(float value, StatModType type, StatParam param, int order, string source)
    {
        Value = value;
        Type = type;
        Param = param;
        Order = order;
        Source = source;
    }

    public StatModifier(float value, StatModType type, StatParam param) : this(value, type, param, (int)type, null) { }

    public StatModifier(float value, StatModType type, StatParam param, int order) : this(value, type, param, order, null) { }

    public StatModifier(float value, StatModType type, StatParam param, string source) : this(value, type, param, (int)type, source) { }

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
        string parameter = "";
        switch (Param)
        {
            case StatParam.AttackSpeed:
                parameter = "Attack Speed";
                break;
            case StatParam.AttackDamage:
                parameter = "Attack Damage";
                break;
            case StatParam.AttackCone:
                parameter = "Attack Cone";
                break;
            case StatParam.AttackRange:
                parameter = "Attack Range";
                break;
            case StatParam.PlayerMoveSpeed:
                parameter = "Move Speed";
                break;
            case StatParam.PlayerMaxHealth:
                parameter = "Max Health";
                break;
            case StatParam.PierceCount:
                parameter = "Pierce Count";
                break;
            case StatParam.ProjectileSpeed:
                parameter = "Projectile Speed";
                break;
            default:
                break;
        }
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