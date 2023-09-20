using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbilityBase : ScriptableObject
{
    public string AbilityName;
    public float AbilityCooldown;
    public List<Stat> Stats;
    public GameObject AbilityGraphics;
    public LayerMask TargetLayer;
    public abstract void Use(Transform source);

    public Stat GetStat(StatParam param)
    {
        return Stats.Find(x => x.Parameter == param);
    }
}
