using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Effects/ShatterEffect")]
public class ShatterEffectSO : EffectSO
{
    public float RecvDamageAmpPerc;
    public override TimedEffect InitializeEffect(NewEnemyBase enemy, object auxData = null)
    {
        return new TimedShatterEffect(this, enemy);
    }
}
