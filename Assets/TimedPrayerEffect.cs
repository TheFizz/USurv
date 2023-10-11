using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedPrayerEffect : TimedEffect
{
    NewEnemyBase _enemy;
    private bool isApplied;
    private float _ampDelta;

    public TimedPrayerEffect(EffectSO EffectData, NewEnemyBase enemy) : base(EffectData, enemy)
    {
        _enemy = enemy;
    }

    protected override void ApplyEffect()
    {
    }

    public override void End()
    {
        var damage = ((PrayerEffectSO)EffectData).DamagePerStack * EffectStacks;
        _enemy.Damage(damage,false);
    }
}
