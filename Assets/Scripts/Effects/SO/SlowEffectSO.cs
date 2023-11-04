using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Effects/SlowEffect")]
public class SlowEffectSO : EffectSO
{
    public float SlowAmountPerc;
    public override TimedEffect InitializeEffect(IEffectable target, object auxData = null)
    {
        if (target is not IMoving)
            throw new InvalidCastException("The object does not implement IMoving.");
        return new TimedSlowEffect(this, (IMoving)target);
    }
}
