using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedCrippleEffect : TimedEffect
{
    NewEnemyBase _enemy;
    private bool isApplied;
    private float _multDelta;

    public TimedCrippleEffect(EffectSO EffectData, NewEnemyBase enemy) : base(EffectData, enemy)
    {
        _enemy = enemy;
    }

    protected override void ApplyEffect()
    {
        CrippleEffectSO crippleEffect = (CrippleEffectSO)EffectData;
        if (!isApplied)
            _multDelta = crippleEffect.DamageMultiplierPerc / 100;
        _enemy.DamageMult += _multDelta;
        isApplied = true;
    }

    public override void End()
    {
        _enemy.DamageMult -= (_multDelta * EffectStacks);
        EffectStacks = 0;
        isApplied = false;
    }
}
