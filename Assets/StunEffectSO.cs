using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Effects/StunEffect")]
public class StunEffectSO : EffectSO
{
    public float StunDuration;
    public override TimedEffect InitializeEffect(NewEnemyBase enemy, object auxData = null)
    {
        return new TimedStunEffect(this, enemy);
    }
}
