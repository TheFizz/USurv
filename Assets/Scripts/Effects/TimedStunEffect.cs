using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedStunEffect : TimedEffect
{
    IStunnable _target;
    public TimedStunEffect(EffectSO EffectData, IStunnable target) : base(EffectData)
    {
        _target = target;
    }

    protected override void ApplyEffect()
    {
        _target.SetStunned(true);
    }

    public override void End()
    {
        _target.SetStunned(false);
    }
}
