using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedShatterEffect : TimedEffect
{
    NewEnemyBase _enemy;
    private bool isApplied;
    private float _ampDelta;

    public TimedShatterEffect(EffectSO EffectData, NewEnemyBase enemy) : base(EffectData, enemy)
    {
        _enemy = enemy;
    }

    protected override void ApplyEffect()
    {
        ShatterEffectSO shatterEffect = (ShatterEffectSO)EffectData;
        if (!isApplied)
            _ampDelta = shatterEffect.RecvDamageAmpPerc / 100;
        _enemy.RecvDamageAmp += _ampDelta;
        isApplied = true;
    }

    public override void End()
    {
        _enemy.RecvDamageAmp -= (_ampDelta * EffectStacks);
        EffectStacks = 0;
        isApplied = false;
    }
}
