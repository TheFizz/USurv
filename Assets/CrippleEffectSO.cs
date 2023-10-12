using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Effects/CrippleEffect")]
public class CrippleEffectSO : EffectSO
{
    public float DamageMultiplierPerc;
    public override TimedEffect InitializeEffect(NewEnemyBase enemy, object auxData = null)
    {
        return new TimedCrippleEffect(this, enemy);
    }
}
