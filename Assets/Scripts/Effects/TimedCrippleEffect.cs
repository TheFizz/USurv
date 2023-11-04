using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedCrippleEffect : TimedEffect
{
    IDamaging _target;
    private bool isApplied;
    private float _multDelta;

    public TimedCrippleEffect(EffectSO EffectData, IDamaging target) : base(EffectData)
    {
        _target = target;
    }

    protected override void ApplyEffect()
    {
        CrippleEffectSO crippleEffect = (CrippleEffectSO)EffectData;
        if (!isApplied)
            _multDelta = crippleEffect.DamageMultiplierPerc / 100;
        _target.OutDmgFactor += _multDelta;
        isApplied = true;
    }

    public override void End()
    {
        _target.OutDmgFactor -= (_multDelta * EffectStacks);
        EffectStacks = 0;
        isApplied = false;
    }
}
