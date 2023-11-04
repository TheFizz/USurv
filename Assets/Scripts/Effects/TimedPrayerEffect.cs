using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedPrayerEffect : TimedEffect
{
    IDamageable _target;
    private bool isApplied;
    private float _ampDelta;

    public TimedPrayerEffect(EffectSO EffectData, IDamageable target) : base(EffectData)
    {
        _target = target;
    }

    protected override void ApplyEffect()
    {
    }

    public override void End()
    {
        var damage = ((PrayerEffectSO)EffectData).DamagePerStack * EffectStacks;
        _target.Damage(damage, false, "PLAYER");
    }
}
