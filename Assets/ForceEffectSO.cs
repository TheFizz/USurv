using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Effects/ForceEffect")]
public class ForceEffectSO : EffectSO
{
    public override TimedEffect InitializeEffect(NewEnemyBase enemy, object auxData = null)
    {
        return new TimedForceEffect(this, enemy, (ForceData)auxData);
    }
}
