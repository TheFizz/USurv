using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Effects/PrayerEffect")]
public class PrayerEffectSO : EffectSO
{
    public float DamagePerStack;
    public override TimedEffect InitializeEffect(IEffectable target, object auxData = null)
    {
        if (target is not IDamageable)
            throw new InvalidCastException("The object does not implement IDamageable.");
        return new TimedPrayerEffect(this, (IDamageable)target);
    }
}
