using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Effects/StunEffect")]
public class StunEffectSO : EffectSO
{
    public override TimedEffect InitializeEffect(IEffectable target, object auxData = null)
    {
        if (target is not IStunnable)
            throw new InvalidCastException("The object does not implement IStunnable.");
        return new TimedStunEffect(this, (IStunnable)target);
    }
}
