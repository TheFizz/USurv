using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedSlowEffect : TimedEffect
{
    IMoving _target;
    float _speedDelta;
    bool isApplied = false;
    public TimedSlowEffect(EffectSO EffectData, IMoving target) : base(EffectData)
    {
        _target = target;
    }

    protected override void ApplyEffect()
    {
        SlowEffectSO slowEffect = (SlowEffectSO)EffectData;
        if (!isApplied)
            _speedDelta = (_target.MoveSpeed * (slowEffect.SlowAmountPerc / 100));
        _target.MoveSpeed -= _speedDelta;
        isApplied = true;
    }

    public override void End()
    {
        _target.MoveSpeed += (_speedDelta * EffectStacks);
        EffectStacks = 0;
        isApplied = false;
    }
}
