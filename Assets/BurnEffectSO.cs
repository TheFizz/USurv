using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Effects/BurnEffect")]
public class BurnEffectSO : EffectSO
{
    public float DamagePerProc;
    public override TimedEffect InitializeEffect(NewEnemyBase enemy, object auxData = null)
    {
        return new TimedBurnEffect(this, enemy);
    }
}
