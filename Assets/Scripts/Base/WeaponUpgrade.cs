using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public class WeaponUpgrade 
{
    public int UpgradeNumber;
    public List<StatModifier> SelfStatMods;
    public List<StatModifier> PassiveStatMods;
    public List<StatModifier> AbilityStatMods;
}