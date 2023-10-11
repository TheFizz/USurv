using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Effects/ShatterEffect")]
public class ShatterEffectSO : EffectSO
{
    public float RecvDamageAmpPerc;
    public override TimedEffect InitializeEffect(NewEnemyBase enemy)
    {
        return new TimedShatterEffect(this, enemy);
    }
}
