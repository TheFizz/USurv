using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedStunEffect : TimedEffect
{
    NewEnemyBase _enemy;
    public TimedStunEffect(EffectSO EffectData, NewEnemyBase enemy) : base(EffectData, enemy)
    {
        _enemy = enemy;
    }

    protected override void ApplyEffect()
    {
        StunEffectSO stunEffect = (StunEffectSO)EffectData;
        _enemy.Stun(stunEffect.StunDuration);
    }

    public override void End()
    {
    }
}
