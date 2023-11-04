using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Effects/ShatterEffect")]
public class ShatterEffectSO : EffectSO
{
    public float RecvDamageAmpPerc;
    public override TimedEffect InitializeEffect(IEffectable target, object auxData = null)
    {
        if (target is not IDamageable)
            throw new InvalidCastException("The object does not implement IDamageable.");

        return new TimedShatterEffect(this, (IDamageable)target);
    }
}
