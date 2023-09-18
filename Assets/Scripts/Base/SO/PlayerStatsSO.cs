 
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public class PlayerStatsSO : ScriptableObject
{
    public List<Stat> PlayerStats = new List<Stat>()
    {
        new Stat(1,StatParam.AttackSpeed),
        new Stat(1,StatParam.AttackRange),
        new Stat(1,StatParam.AttackDamage),
        new Stat(1,StatParam.AttackCone),
        new Stat(100,StatParam.PlayerMaxHealth),
        new Stat(7,StatParam.PlayerMoveSpeed)
    };
    public float XPThresholdBase = 12;
    public float XPThresholdMultiplier = 1.2f;
    public Stat GetStat(StatParam param)
    {
        return PlayerStats.FirstOrDefault<Stat>(x => x.Parameter == param);
    }
}
