using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class WeaponUpgradeSO : ScriptableObject
{
    public int UpgradeNumber;
    public List<StatModifier> SelfStatMods;
    public List<StatModifier> PassiveStatMods;
    public List<StatModifier> AbilityStatMods;
}
