 
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public class PlayerStatsSO : ScriptableObject
{
    public List<Stat> Stats;
    public float XPThresholdBase = 12;
    public float XPThresholdMultiplier = 1.2f;
    public Stat GetStat(StatParam param)
    {
        return Stats.FirstOrDefault<Stat>(x => x.Parameter == param);
    }
}
