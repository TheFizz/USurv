using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GlobalUpgrade
{
    [HideInInspector] public int UpgradeNumber = 0;
    public StatParam UpgradeParam;
    public List<StatModifier> Modifiers;
}
