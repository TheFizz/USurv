using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Effects/SlowEffect")]
public class SlowEffectSO : EffectSO
{
    public float SlowAmountPerc;
    public override TimedEffect InitializeEffect(NewEnemyBase enemy)
    {
        return new TimedSlowEffect(this, enemy);
    }
}
