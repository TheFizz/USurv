using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBow : WeaponBaseRanged
{
    public override List<StatModifier> WeaponModifiers { get; } = new List<StatModifier>()
    {
        new StatModifier(50, StatModType.Percent, StatModParameter.AttackSpeed)
    };
}
