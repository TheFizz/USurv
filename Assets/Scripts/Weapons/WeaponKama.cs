using Kryz.CharacterStats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponKama : WeaponBaseMelee
{
    public override List<StatModifier> WeaponModifiers { get; } = new List<StatModifier>()
    {
        new StatModifier(50,StatModType.PercentMult, StatModParam.AttackDamage)
    };
}
