using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Effects/CrippleEffect")]
public class CrippleEffectSO : EffectSO
{
    public float DamageMultiplierPerc;
    public override TimedEffect InitializeEffect(IEffectable target, object auxData = null)
    {
        if (target is not IDamaging)
            throw new InvalidCastException("The object does not implement IDamaging.");
        return new TimedCrippleEffect(this, (IDamaging)target);
    }
}
