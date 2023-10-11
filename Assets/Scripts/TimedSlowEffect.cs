using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedSlowEffect : TimedEffect
{
    NewEnemyBase _enemy;
    float _speedDelta;
    bool isApplied = false;
    public TimedSlowEffect(EffectSO EffectData, NewEnemyBase enemy) : base(EffectData, enemy)
    {
        _enemy = enemy;
    }

    protected override void ApplyEffect()
    {
        SlowEffectSO slowEffect = (SlowEffectSO)EffectData;
        if (!isApplied)
            _speedDelta = (_enemy.BaseSpeed * (slowEffect.SlowAmountPerc / 100));
        _enemy.BaseSpeed -= _speedDelta;
        isApplied = true;
    }

    public override void End()
    {
        _enemy.BaseSpeed += (_speedDelta * EffectStacks);
        EffectStacks = 0;
        isApplied = false;
    }
}
