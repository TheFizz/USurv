using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatModType
{
    Flat,
    Percent,
}
public enum StatModParameter
{
    AttackSpeed,
    AttackDamage,
    AttackArc,
    AttackRange,
    PlayerMoveSpeed
}
public class StatModifier
{
    public float Value { get; }
    public StatModType Type { get; }
    public StatModParameter Parameter { get; }
    public StatModifier(float value, StatModType type, StatModParameter parameter)
    {
        Value = value;
        Type = type;
        Parameter = parameter;
    }
}
