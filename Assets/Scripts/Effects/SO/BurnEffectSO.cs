using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Effects/BurnEffect")]
public class BurnEffectSO : EffectSO
{
    public float DamagePerProc;
    public override TimedEffect InitializeEffect(IEffectable target, object auxData = null)
    {
        if (target is not IDamageable)
            throw new InvalidCastException("The object does not implement IDamageable.");
        return new TimedBurnEffect(this, (IDamageable)target);
    }
}
