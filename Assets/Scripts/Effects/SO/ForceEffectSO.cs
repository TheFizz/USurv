using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Effects/ForceEffect")]
public class ForceEffectSO : EffectSO
{
    public override TimedEffect InitializeEffect(IEffectable target, object auxData = null)
    {
        if (target is not IForceable)
            throw new InvalidCastException("The object does not implement IForceable.");
        return new TimedForceEffect(this, (IForceable)target, (ForceData)auxData);
    }
}
