using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedBurnEffect : TimedEffect
{
    NewEnemyBase _enemy;
    public TimedBurnEffect(EffectSO EffectData, NewEnemyBase enemy) : base(EffectData, enemy)
    {
        _enemy = enemy;
    }

    protected override void ApplyEffect()
    {
    }

    public override void End()
    {
    }
    protected override void Proc()
    {
        BurnEffectSO burnEffect = (BurnEffectSO)EffectData;
        _enemy.Damage(burnEffect.DamagePerProc, false);
    }
}
