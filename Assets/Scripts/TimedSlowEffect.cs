using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedSlowEffect : TimedEffect
{
    NewEnemyBase _enemy;
    float _speedDelta;
    public TimedSlowEffect(EffectSO Effect, NewEnemyBase enemy) : base(Effect, enemy)
    {
        _enemy = enemy;
    }

    protected override void ApplyEffect()
    {
        SlowEffectSO slowEffect = (SlowEffectSO)EffectData;
        _speedDelta = (_enemy.BaseSpeed * (slowEffect.SlowAmountPerc / 100));
        _enemy.BaseSpeed -= _speedDelta;
    }

    public override void End()
    {
        SlowEffectSO slowEffect = (SlowEffectSO)EffectData;
        EffectStacks = 0;
        _enemy.BaseSpeed += _speedDelta;
    }
}
