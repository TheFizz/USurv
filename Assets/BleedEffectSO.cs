using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Effects/BleedEffect")]
public class BleedEffectSO : EffectSO
{
    public float DamagePerStack;
    public override TimedEffect InitializeEffect(NewEnemyBase enemy)
    {
        return new TimedBleedEffect(this, enemy);
    }
}
